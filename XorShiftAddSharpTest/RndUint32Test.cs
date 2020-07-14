using System;
using Xunit;
using Xunit.Abstractions;

namespace XorShiftAddSharpTest
{
    public class RndUint32Test
    {
        private readonly ITestOutputHelper _output;
        public RndUint32Test(ITestOutputHelper output) => _output = output;

        private static void Assert(uint seed, ReadOnlySpan<uint> expected)
        {
            var vec = new uint[4];


            XorShiftAddSharp.XSAddCore.xsadd_init(vec, seed);

        }

        [Fact]
        public void RndTest()
        {
            

        }

    }
}
