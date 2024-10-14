<<<<<<< HEAD
using Miningcore.Native;
using static Miningcore.Native.Cryptonight.Algorithm;
=======
using Miningcore.Contracts;
using Miningcore.Native;
>>>>>>> 69de0d393ec56f3e0535f3b09f6de93d6299beec

namespace Miningcore.Crypto.Hashing.Algorithms;

[Identifier("flex")]
<<<<<<< HEAD
public class Flex : IHashAlgorithm
{
    public void Digest(ReadOnlySpan<byte> data, Span<byte> result, params object[] extra)
    {
        Cryptonight.CryptonightHash(data, result, FLEX_KCN, 0);
=======

public unsafe class Flex : IHashAlgorithm
{
    public void Digest(ReadOnlySpan<byte> data, Span<byte> result, params object[] extra)
    {
        Contract.Requires<ArgumentException>(result.Length >= 32);

        fixed (byte* input = data)
        {
            fixed (byte* output = result)
            {
                Multihash.flex(input, output);
            }
        }
>>>>>>> 69de0d393ec56f3e0535f3b09f6de93d6299beec
    }
}
