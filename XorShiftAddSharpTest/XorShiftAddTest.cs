using ChainingAssertion;
using System;
using XorShiftAddSharp;
using Xunit;
using Xunit.Abstractions;

namespace XorShiftAddSharpTest
{
    public class XorShiftAddTest
    {
        private readonly ITestOutputHelper _output;

        public XorShiftAddTest(ITestOutputHelper output) => _output = output;


        static void Assert(ReadOnlySpan<uint> expected, XorShiftAdd actual)
        {
            actual.State.Count.Is(4);

            for (int i = 0; i < expected.Length; i++)
            {
                expected[i].Is(actual.State[i]);
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

            Span<uint> expected = stackalloc uint[4];

            XorShiftAddCore.Init(expected, 42);
            var actual = new XorShiftAdd(42);
            Assert(expected, actual);

            var keys = new[] { 4u, 13u, 930u, 3767u, 31980u, 967285u, 3690813u, 85575140u, 106037230u, 3994571597u, };

            XorShiftAddCore.Init(expected, keys);
            actual = new XorShiftAdd(keys);
            Assert(expected, actual);
        }



    }
}
