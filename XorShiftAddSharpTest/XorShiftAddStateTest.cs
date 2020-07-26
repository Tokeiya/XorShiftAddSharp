using System;
using System.Collections.Generic;
using System.Linq;
using ChainingAssertion;
using XorShiftAddSharp;
using Xunit;
using Xunit.Abstractions;
//using static Xunit.Assert;

namespace XorShiftAddSharpTest
{
    public class XorShiftAddStateTest
    {
        private readonly ITestOutputHelper _output;

        public XorShiftAddStateTest(ITestOutputHelper output) => _output = output;

        [Fact]
        public void ConstSizeTest()
        {
            InternalState.Size.Is(4);
        }

        [Fact]
        public void InvalidInitializeTest()
        {
            var array = new uint[10];

            for (int i = 0; i < array.Length; i++)
            {
                if(i==InternalState.Size) continue;
                Assert.Throws<ArgumentException>(() => InternalState.Initialize(new ReadOnlySpan<uint>(array, 0, i)));

                var ary = array.Take(i).ToList();
                Assert.Throws<ArgumentException>(() => InternalState.Initialize(ary));
            }
        }

        [Fact]
        public void InitializeTest()
        {
            static void assert(InternalState actual, ReadOnlySpan<uint> expected)
            {
                for (var i = 0; i < InternalState.Size; ++i) actual[i].Is(expected[i]);
            }

            var array = Enumerable.Range(0, 4).Select(x => (uint) (x + 1)).ToArray();

            var actual = InternalState.Initialize((IReadOnlyList<uint>) array);
            assert(actual, array);

            actual = InternalState.Initialize(new ReadOnlySpan<uint>(array));
            assert(actual, array);
        }


        [Fact]
        public void IndexerIndexOutOfRangeTest()
        {
            static void Check(InternalState actual,int index)
            {
                uint dmy = 0;
                Assert.Throws<IndexOutOfRangeException>(() => dmy = actual[index]);
                Assert.Throws<IndexOutOfRangeException>(() => actual[index] = 10);
            }
            var actual = new InternalState();

            Check(actual, -1);
            Check(actual, 4);
        }

        [Fact]
        public unsafe void IndexerGetTest()
        {
            var actual = new InternalState();

            for (uint i = 0; i < InternalState.Size; i++)
            {
                actual.State[i] = i + 10u;
            }

            for (int i = 0; i < InternalState.Size; i++)
            {
                actual[i].Is(actual.State[i]);
            }
        }

        [Fact]
        public unsafe void IndexerSetTest()
        {
            var actual=new InternalState();

            for (int i = 0; i < InternalState.Size; i++)
            {
                actual[i].Is(0u);
                actual[i] = (uint)i + 42;
                actual[i].Is((uint)i + 42);
            }
        }

        [Fact]
        public void CopyToTest()
        {
            Assert.True(false);
        }

        [Fact]
        public void ToArrayTest()
        {
            Assert.True(false);
        }

    }
}
