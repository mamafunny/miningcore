using System.Data;
using System.Data.Common;
using System.Net.Sockets;
using Miningcore.Configuration;
using Miningcore.Extensions;
using Miningcore.Mining;
using Miningcore.Persistence;
using Miningcore.Persistence.Model;
using Miningcore.Persistence.Repositories;
using Miningcore.Util;
using NLog;
using Polly;
using Contract = Miningcore.Contracts.Contract;

namespace Miningcore.Payments.PaymentSchemes;


/// <summary>
/// PPLNSBF payout scheme implementation
/// </summary>
// ReSharper disable once InconsistentNaming
public class PPLNSBFPaymentScheme : IPayoutScheme
{
    public PPLNSBFPaymentScheme(
        IConnectionFactory cf,
        IShareRepository shareRepo,
        IBlockRepository blockRepo,
        IBalanceRepository balanceRepo)
    {
        Contract.RequiresNonNull(cf);
        Contract.RequiresNonNull(shareRepo);
        Contract.RequiresNonNull(blockRepo);
        Contract.RequiresNonNull(balanceRepo);

        this.cf = cf;
        this.shareRepo = shareRepo;
        this.blockRepo = blockRepo;
        this.balanceRepo = balanceRepo;

        BuildFaultHandlingPolicy();
    }

    private readonly IBalanceRepository balanceRepo;
    private readonly IBlockRepository blockRepo;
    private readonly IConnectionFactory cf;
    private readonly IShareRepository shareRepo;
    private static readonly ILogger logger = LogManager.GetLogger("PPLNSBF Payment");

    private const int RetryCount = 4;
    private IAsyncPolicy shareReadFaultPolicy;

    private class Config
    {
        public decimal Factor { get; set; }
    }

    #region IPayoutScheme

 public async Task UpdateBalancesAsync(IDbConnection con, IDbTransaction tx, IMiningPool pool, IPayoutHandler payoutHandler,
    Block block, decimal blockReward, CancellationToken ct)
{
    var poolConfig = pool.Config;
    var payoutConfig = poolConfig.PaymentProcessing.PayoutSchemeConfig;

    // PPLNS window (see https://bitcointalk.org/index.php?topic=39832)
    var window = payoutConfig?.ToObject<Config>()?.Factor ?? 2.0m;

    // Calculate the block finder reward (10% of the block reward)
    var blockFinderReward = blockReward * 0.1m;

    // Calculate the total reward for miners (excluding block finder reward)
    var totalRewardForMiners = blockReward - blockFinderReward;

    // calculate rewards
    var shares = new Dictionary<string, double>();
    var rewards = new Dictionary<string, decimal>();
    var shareCutOffDate = await CalculateRewardsAsync(pool, payoutHandler, window, block, totalRewardForMiners, shares, rewards, ct);

    // Update balances for miner rewards
    foreach (var address in rewards.Keys)
    {
        var amount = rewards[address];

        if (amount > 0)
        {
            logger.Info(() => $"Crediting {address} with {payoutHandler.FormatAmount(amount)} for {FormatUtil.FormatQuantity(shares[address])} ({shares[address]}) shares for block {block.BlockHeight}");
            await balanceRepo.AddAmountAsync(con, tx, poolConfig.Id, address, amount, $"Reward for {FormatUtil.FormatQuantity(shares[address])} shares for block {block.BlockHeight}");
        }
    }

    // Handle block finder reward separately
    logger.Info(() => $"Block finder reward: {payoutHandler.FormatAmount(blockFinderReward)} for block {block.BlockHeight} mined by {block.Miner}");
    await balanceRepo.AddAmountAsync(con, tx, poolConfig.Id, block.Miner, blockFinderReward, $"Block finder reward for block {block.BlockHeight}");

    // Delete discarded shares
    if (shareCutOffDate.HasValue)
    {
        var cutOffCount = await shareRepo.CountSharesBeforeAsync(con, tx, poolConfig.Id, shareCutOffDate.Value, ct);

        if (cutOffCount > 0)
        {
            await LogDiscardedSharesAsync(ct, poolConfig, block, shareCutOffDate.Value);

            logger.Info(() => $"Deleting {cutOffCount} discarded shares before {shareCutOffDate.Value:O}");
            await shareRepo.DeleteSharesBeforeAsync(con, tx, poolConfig.Id, shareCutOffDate.Value, ct);
        }
    }

 // Diagnostics
var totalShareCount = shares.Values.ToList().Sum(x => new decimal(x));
var totalRewards = rewards.Values.ToList().Sum(x => x); // Total rewards distributed to miners

// Add the block finder reward to the total rewards
totalRewards += blockFinderReward;

if (totalRewards > 0)
    logger.Info(() => $"{FormatUtil.FormatQuantity((double)totalShareCount)} ({Math.Round(totalShareCount, 2)}) shares contributed to a total payout of {payoutHandler.FormatAmount(totalRewards)} ({totalRewards / totalRewardForMiners * 100:0.00}% of block reward) to {rewards.Keys.Count + 1} addresses");

 }


    private async Task LogDiscardedSharesAsync(CancellationToken ct, PoolConfig poolConfig, Block block, DateTime value)
    {
        var before = value;
        var pageSize = 50000;
        var currentPage = 0;
        var shares = new Dictionary<string, double>();

        while (true)
        {
            logger.Info(() => $"Fetching page {currentPage} of discarded shares for pool {poolConfig.Id}, block {block.BlockHeight}");

            var page = await shareReadFaultPolicy.ExecuteAsync(() =>
                cf.Run(con => shareRepo.ReadSharesBeforeAsync(con, poolConfig.Id, before, false, pageSize, ct)));

            currentPage++;

            for (var i = 0; i < page.Length; i++)
            {
                var share = page[i];
                var address = share.Miner;

                // record attributed shares for diagnostic purposes
                if (!shares.ContainsKey(address))
                    shares[address] = share.Difficulty;
                else
                    shares[address] += share.Difficulty;
            }

            if (page.Length < pageSize)
                break;

            before = page[^1].Created;
        }

        if (shares.Keys.Count > 0)
        {
            // sort addresses by shares
            var addressesByShares = shares.Keys.OrderByDescending(x => shares[x]);

            logger.Info(() => $"{FormatUtil.FormatQuantity(shares.Values.Sum())} ({shares.Values.Sum()}) total discarded shares, block {block.BlockHeight}");

            foreach (var address in addressesByShares)
                logger.Info(() => $"{address} = {FormatUtil.FormatQuantity(shares[address])} ({shares[address]}) discarded shares, block {block.BlockHeight}");
        }
    }

    #endregion // IPayoutScheme

    private async Task<DateTime?> CalculateRewardsAsync(IMiningPool pool, IPayoutHandler payoutHandler, decimal window, Block block, decimal blockReward,
        Dictionary<string, double> shares, Dictionary<string, decimal> rewards, CancellationToken ct)
    {
        var poolConfig = pool.Config;
        var done = false;
        var before = block.Created;
        var inclusive = true;
        var pageSize = 50000;
        var currentPage = 0;
        var accumulatedScore = 0.0m;
        var blockRewardRemaining = blockReward;
        DateTime? shareCutOffDate = null;

        // Calculate the block finder reward (10% of the block reward)
        var blockFinderReward = blockReward * 0.1m;

        while (!done && !ct.IsCancellationRequested)
        {
            logger.Info(() => $"Fetching page {currentPage} of shares for pool {poolConfig.Id}, block {block.BlockHeight}");

            var page = await shareReadFaultPolicy.ExecuteAsync(() =>
                cf.Run(con => shareRepo.ReadSharesBeforeAsync(con, poolConfig.Id, before, inclusive, pageSize, ct))); //, sw, logger));

            inclusive = false;
            currentPage++;

            for (var i = 0; !done && i < page.Length; i++)
            {
                var share = page[i];

                var address = share.Miner;
                var shareDiffAdjusted = payoutHandler.AdjustShareDifficulty(share.Difficulty);

                // record attributed shares for diagnostic purposes
                if (!shares.ContainsKey(address))
                    shares[address] = shareDiffAdjusted;
                else
                    shares[address] += shareDiffAdjusted;

                var score = (decimal)(shareDiffAdjusted / share.NetworkDifficulty);

                // if accumulated score would cross threshold, cap it to the remaining value
                if (accumulatedScore + score >= window)
                {
                    score = window - accumulatedScore;
                    shareCutOffDate = share.Created;
                    done = true;
                }

                // Calculate reward excluding block finder reward
                var reward = score * (blockReward - blockFinderReward) / window;
                accumulatedScore += score;
                blockRewardRemaining -= reward;

                // this should never happen
                if (blockRewardRemaining <= 0 && !done)
                    throw new OverflowException("blockRewardRemaining < 0");

                if (reward > 0)
                {
                    // accumulate miner reward
                    if (!rewards.ContainsKey(address))
                        rewards[address] = reward;
                    else
                        rewards[address] += reward;
                }
            }

            if (page.Length < pageSize)
                break;

            before = page[^1].Created;
        }

        logger.Info(() => $"Balance-calculation for pool {poolConfig.Id}, block {block.BlockHeight} completed with accumulated score {accumulatedScore:0.####} ({(accumulatedScore / window) * 100:0.#}%)");

        // Log the block finder reward
        logger.Info(() => $"Block finder reward: {payoutHandler.FormatAmount(blockFinderReward)} for block {block.BlockHeight} mined by {block.Miner}");

        // Give block finder reward
        var blockFinderAddress = block.Miner;
        if (!rewards.ContainsKey(blockFinderAddress))
            rewards[blockFinderAddress] = blockFinderReward;
        else
            rewards[blockFinderAddress] += blockFinderReward;

        return shareCutOffDate;
    }

    private void BuildFaultHandlingPolicy()
    {
        var retry = Policy
            .Handle<DbException>()
            .Or<SocketException>()
            .Or<TimeoutException>()
            .RetryAsync(RetryCount, OnPolicyRetry);

        shareReadFaultPolicy = retry;
    }

    private static void OnPolicyRetry(Exception ex, int retry, object context)
    {
        logger.Warn(() => $"Retry {retry} due to {ex.Source}: {ex.GetType().Name} ({ex.Message})");
    }
}

