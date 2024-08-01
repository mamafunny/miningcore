using System.Globalization;
using System.Numerics;
using System.Text.RegularExpressions;

// ReSharper disable InconsistentNaming

namespace Miningcore.Blockchain.Kaspa;

public static class BugnaConstants
{
    // List of BGA prefixes: https://github.com/bugnanetwork/bugnad/blob/master/util/address.go
    public const string ChainPrefixDevnet = "bugnadev";
    public const string ChainPrefixSimnet = "bugnasim";
    public const string ChainPrefixTestnet = "bugnatest";
    public const string ChainPrefixMainnet = "bugna";
}

public static class HoosatConstants
{
    // List of HTN prefixes: https://github.com/Hoosat-Oy/HTND/blob/master/util/address.go
    public const string ChainPrefixDevnet = "htndev";
    public const string ChainPrefixSimnet = "hoosatsim";
    public const string ChainPrefixTestnet = "hoosattest";
    public const string ChainPrefixMainnet = "hoosat";
}

public static class KobradagConstants
{
    // List of Kobra prefixes: 
    public const string ChainPrefixDevnet = "kobradev";
    public const string ChainPrefixSimnet = "kobrasim";
    public const string ChainPrefixTestnet = "kobratest";
    public const string ChainPrefixMainnet = "kobra";
}

public static class KaspaConstants
{
    public const string WalletDaemonCategory = "wallet";
    
    public const int Diff1TargetNumZero = 31;
    public static readonly BigInteger Diff1b = BigInteger.Parse("00000000ffff0000000000000000000000000000000000000000000000000000", NumberStyles.HexNumber);
    public static BigInteger Diff1 = BigInteger.Pow(2, 256);
    public static BigInteger Diff1Target = BigInteger.Pow(2, 255) - 1;
    public static readonly double Pow2xDiff1TargetNumZero = Math.Pow(2, Diff1TargetNumZero);
    public static BigInteger BigOne = BigInteger.One;
    public static BigInteger OneLsh256 = BigInteger.One << 256;
    public static BigInteger MinHash = BigInteger.Divide(Diff1, Diff1Target);
    public static BigInteger BigGig = BigInteger.Pow(10, 9);
    public const int ExtranoncePlaceHolderLength = 8;
    public static int NonceLength = 16;
    public const uint ShareMultiplier = 1;
    
    // KAS smallest unit is called SOMPI: https://github.com/kaspanet/kaspad/blob/master/util/amount.go
    public const decimal SmallestUnit = 100000000;

    // List of KAS prefixes: https://github.com/kaspanet/kaspad/blob/master/util/address.go
    public const string ChainPrefixDevnet = "kaspadev";
    public const string ChainPrefixSimnet = "kaspasim";
    public const string ChainPrefixTestnet = "kaspatest";
    public const string ChainPrefixMainnet = "kaspa";

    public static readonly Regex RegexUserAgentBzMiner = new("bzminer", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    public static readonly Regex RegexUserAgentIceRiverMiner = new("iceriverminer", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    public static readonly Regex RegexUserAgentGodMiner = new("godminer", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    
    public const string CoinbaseBlockHash = "BlockHash";
    public const string CoinbaseProofOfWorkHash = "ProofOfWorkHash";
    public const string CoinbaseHeavyHash = "HeavyHash";
    
    public const string ProtobufDaemonRpcServiceName = "protowire.RPC";
    public const string ProtobufWalletRpcServiceName = "kaspawalletd.kaspawalletd";
    
    public const byte PubKeyAddrID = 0x00;
    public const byte PubKeyECDSAAddrID = 0x01;
    public const byte ScriptHashAddrID = 0x08;
    public static readonly Dictionary<byte, string> KaspaAddressType = new Dictionary<byte, string>
    {
        { PubKeyAddrID, "Public Key Address" },
        { PubKeyECDSAAddrID, "Public Key ECDSA Address" },
        { ScriptHashAddrID, "Script Hash Address" },
    };
    public const int PublicKeySize = 32;
    public const int PublicKeySizeECDSA = 33;
    public const int Blake2bSize256 = 32;
}

public static class KaspaClassicConstants
{
    // List of CAS prefixes: https://github.com/kaspaclassic/caspad/blob/main/util/address.go
    public const string ChainPrefixDevnet = "caspadev";
    public const string ChainPrefixSimnet = "pyrinsim";
    public const string ChainPrefixTestnet = "pyrintest";
    public const string ChainPrefixMainnet = "cas";
}

public static class KarlsencoinConstants
{
    // List of KLS prefixes: https://github.com/karlsen-network/karlsend/blob/master/util/address.go
    public const string ChainPrefixDevnet = "karlsendev";
    public const string ChainPrefixSimnet = "karlsensim";
    public const string ChainPrefixTestnet = "karlsentest";
    public const string ChainPrefixMainnet = "karlsen";
    
    public const long FishHashForkHeightTestnet = 0;
    public const long FishHashPlusForkHeightTestnet = 6000000;
}

public static class ConsensusConstants
{
    // List of CSS prefixes: https://github.com/consensus-network/consensusd/blob/master/util/address.go
    public const string ChainPrefixDevnet = "consensusdev";
    public const string ChainPrefixSimnet = "consensussim";
    public const string ChainPrefixTestnet = "consensustest";
    public const string ChainPrefixMainnet = "consensus";
}

public static class PugdagConstants
{
    // List of PUG prefixes: https://github.com/Pugdag/pugdagd/blob/main/util/address.go
    public const string ChainPrefixDevnet = "pugdagdev";
    public const string ChainPrefixSimnet = "pugdagsim";
    public const string ChainPrefixTestnet = "pugdagtest";
    public const string ChainPrefixMainnet = "pugdag";
}

public static class NautilusConstants
{
    // List of NTL prefixes: https://github.com/Nautilus-Network/nautiliad/blob/master/util/address.go
    public const string ChainPrefixDevnet = "nautiliadev";
    public const string ChainPrefixSimnet = "nautilussim";
    public const string ChainPrefixTestnet = "nautilustest";
    public const string ChainPrefixMainnet = "nautilus";
}

public static class NexelliaConstants
{
    // List of NXL prefixes: https://github.com/Nexellia-Network/nexelliad/blob/master/util/address.go
    public const string ChainPrefixDevnet = "nexelliadev";
    public const string ChainPrefixSimnet = "nexelliasim";
    public const string ChainPrefixTestnet = "nexelliatest";
    public const string ChainPrefixMainnet = "nexellia";
}

public static class PyrinConstants
{
    // List of KLS prefixes: https://github.com/Pyrinpyi/pyipad/blob/master/util/address.go
    public const string ChainPrefixDevnet = "pyipadev";
    public const string ChainPrefixSimnet = "pyrinsim";
    public const string ChainPrefixTestnet = "pyrintest";
    public const string ChainPrefixMainnet = "pyrin";
    
    public const long Blake3ForkHeight = 1484741;
}

public static class SedraCoinConstants
{
    // List of HTN prefixes: https://github.com/sedracoin/sedrad/blob/main/util/address.go
    public const string ChainPrefixDevnet = "sedradev";
    public const string ChainPrefixSimnet = "sedrasim";
    public const string ChainPrefixTestnet = "sedratest";
    public const string ChainPrefixMainnet = "sedra";
}

public enum KaspaBech32Prefix
{
    Unknown = 0,
    KaspaMain,
    KaspaDev,
    KaspaTest,
    KaspaSim
}