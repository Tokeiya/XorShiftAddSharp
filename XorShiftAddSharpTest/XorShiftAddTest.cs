using ChainingAssertion;
using System;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.ObjectModel;
using XorShiftAddSharp;
using Xunit;
using Xunit.Abstractions;

namespace XorShiftAddSharpTest
{
    public class XorShiftAddTest
    {
        private readonly ITestOutputHelper _output;

        public XorShiftAddTest(ITestOutputHelper output) => _output = output;


        static unsafe void AssertInternalVector(in InternalState expected, XorShiftAdd actual)
        {
	        var state = actual.GetCurrentState();
            for (int i = 0; i < InternalState.Size; i++)
            {
                expected.State[i].Is(state[i]);
            }
        }

        [Fact]
        public void CalculateJumpPolynomialTest()
        {
            static void assert(ReadOnlySpan<char> expected, string actual)
            {
                int i;

                for (i = 0; i < expected.Length; i++)
                {
                    if (expected[i] == '\0') break;
                }

                i.Is(actual.Length);

                for (int j = 0; j < i; j++)
                {
                    expected[j].Is(actual[j]);
                }
            }

            var param = new[]
            {
                (1u, "8"), (73u, "b"), (850u, "67"), (1850u, "de0"), (92775u, "482c"), (286113u, "cb631"),
                (5094526u, "4b2433"), (34793650u, "1feed46"), (218844143u, "1c3f41a5"),
                (1864378911u, "c6c6ce90"),
            };

            Span<char> expected = stackalloc char[33];

            foreach (var elem in param)
            {
                XorShiftAddCore.CalculateJumpPolynomial(expected, elem.Item1, elem.Item2);
                var ret = XorShiftAdd.CalculateJumpPolynomial(elem.Item1, elem.Item2);

                assert(expected, ret);
            }


        }


        [Fact]
        public void CtorTest()
        {

            var expected = new InternalState();

            XorShiftAddCore.Init(out expected, 42);
            var actual = new XorShiftAdd(42);
            AssertInternalVector(expected, actual);

            var keys = new[] { 4u, 13u, 930u, 3767u, 31980u, 967285u, 3690813u, 85575140u, 106037230u, 3994571597u, };

            XorShiftAddCore.Init(out expected, keys);
            actual = new XorShiftAdd(keys);
            AssertInternalVector(expected, actual);
        }

        [Fact]
        public void RestoreTestA()
        {
            var rnd = new XorShiftAdd(42);
            rnd.Next();

            var act = XorShiftAdd.Restore(rnd.GetCurrentState());
            var exp = rnd.GetCurrentState();

            for (int i = 0; i < InternalState.Size; i++)
            {
                act.GetCurrentState()[i].Is(exp[i]);
            }

            for (int i = 0; i < 10; i++)
            {
	            var tmp = rnd.NextUnsignedInt();
	            tmp.Is(act.NextUnsignedInt());
            }
        }

        [Fact]
        public unsafe void TestNameB()
        {
            var expected=new InternalState();
            XorShiftAddCore.Init(out expected, 42);

            var actual = XorShiftAdd.Restore(expected.ToArray());

            for (var i = 0; i < InternalState.Size; ++i)
            {
                actual.GetCurrentState()[i].Is(expected.State[i]);
            }

            for (int i = 0; i < 10; i++)
            {
	            var tmp = XorShiftAddCore.NextUint32(ref expected);
	            actual.NextUnsignedInt().Is(tmp);
            }
        }


        [Fact]
        public void NextUint32Test()
        {
            var rnd = new XorShiftAdd(42);
            var state=new InternalState();
            XorShiftAddCore.Init(out state,42);

            for (int i = 0; i < 100; i++)
            {
                var expected = XorShiftAddCore.NextUint32(ref state);

                rnd.NextUnsignedInt().Is(expected);

            }
        }

        [Fact]
        public void NextFloatTest()
        {
            var rnd = new XorShiftAdd(42);
            var state = new InternalState();
            XorShiftAddCore.Init(out state, 42);

            for (int i = 0; i < 100; i++)
            {
                var expected = XorShiftAddCore.NextFloat(ref state);
                rnd.NextFloat().Is(expected);
            }
        }

        [Fact]
        public void NextTestA()
        {
            var state = new InternalState();
            XorShiftAddCore.Init(out state, 42);

            XorShiftAdd rnd = new XorShiftAdd(42);
            const int maxValue = 100;

            for (int i = 0; i < 100; i++)
            {
                var expected = (int) (XorShiftAddCore.NextDouble(ref state) * maxValue);
                rnd.Next(maxValue).Is(expected);
            }

            Assert.Throws<ArgumentOutOfRangeException>(() => rnd.Next(-1));
        }

        [Fact]
        public void NextTestB()
        {
            var state = new InternalState();
            XorShiftAddCore.Init(out state, 42);

            XorShiftAdd rnd = new XorShiftAdd(42);

            const int minValue = 10;
            const int maxValue = 100;

            for (int i = 0; i < 100; i++)
            {
                var expected = (int) ((long) (XorShiftAddCore.NextDouble(ref state) * (maxValue - minValue)) + minValue);
                var act = rnd.Next(minValue, maxValue);
                act.Is(expected);
            }

            Assert.Throws<ArgumentOutOfRangeException>(() => rnd.Next(100, 10));
        }

        [Fact]
        public unsafe void JumpTestA()
        {
	        static void test(string bs)
	        {
		        const int ms = 100;


		        var rnd = new XorShiftAdd(42);
		        var expected = new InternalState();
		        XorShiftAddCore.Init(out expected, 42);

		        var actual = rnd.Jump(ms, bs);
		        XorShiftAddCore.Jump(ref expected, ms, bs);

		        var state = actual.GetCurrentState();

		        for (int i = 0; i < InternalState.Size; i++)
		        {
			        state[i].Is(expected.State[i]);
		        }

            }

            test("ff");
            test("0xff");
            test("0XFF");
        }

        [Fact]
        public unsafe void JumpTestB()
        {
            const string jumbStr = "5b0abc7da8055ce3aef263ccb271d12e";

            var rnd = new XorShiftAdd(42);

            var expected = new InternalState();
            XorShiftAddCore.Init(out expected, 42);
            XorShiftAddCore.Jump(ref expected, jumbStr);

            var actual = rnd.Jump(jumbStr);
            var state = actual.GetCurrentState();

            for (int i = 0; i < InternalState.Size; i++)
            {
                state[i].Is(expected.State[i]);
            }

        }

        [Fact]
        public void NextDoubleTest()
        {
            var expected = new InternalState();
            XorShiftAddCore.Init(out expected, 114514);

            var rnd = new XorShiftAdd(114514);

            for (int i = 0; i < 100; i++)
            {
                rnd.NextDouble().Is(XorShiftAddCore.NextDouble(ref expected));
            }
        }

        [Fact]
        public void NextBytesTestA()
        {
            var expected = new InternalState();
            XorShiftAddCore.Init(out expected, 114514);

            var rnd = new XorShiftAdd(114514);
            var actual = new byte[128];

            rnd.NextBytes(actual);

            foreach (var i in actual)
            {
                i.Is((byte) XorShiftAddCore.NextUint32(ref expected));
            }
        }

        [Fact]
        public void NextBytesTestB()
        {
            var expected = new InternalState();
            XorShiftAddCore.Init(out expected, 114514);

            var rnd = new XorShiftAdd(114514);
            Span<byte> actual = stackalloc byte[128];

            rnd.NextBytes(actual);

            foreach (var i in actual)
            {
                i.Is((byte)XorShiftAddCore.NextUint32(ref expected));
            }

        }








    }
}
