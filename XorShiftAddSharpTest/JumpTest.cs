using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ChainingAssertion;
using XorShiftAddSharp;
using Xunit;
using Xunit.Abstractions;

namespace XorShiftAddSharpTest
{
    public class JumpTest
    {
        public static IReadOnlyList<(uint mulStep, string baseStep, string expected)> TestSamples =
            new (uint mulStep, string baseStep, string expected)[]
            {
                (1145448892, "cd24fdcd11716aa1", "f1044abf11c45b62106e129c7779e0f4"),
                (2771179739, "e700ac6c84f58579", "1671ade5bd6e8b5043b2c658b18e87de"),
                (1783745685, "b14750a9c0d620d0", "8780c3ff7f0619c8dffa21fe7c045f59"),
                (923673609, "fa3c8b37626cd479", "b97dbd1e1116395af810685d2aa652ec"),
                (3404248797, "7e5305cbc5239792", "1caebf51224af5b4154520b91ca8e287"),
                (2437013609, "3217830da389788f", "ddb43c02eace0477424b417656bb4f57"),
                (132947522, "6f728fd4ab7a87fc", "c4c29c3d3fc1c98a27dad56b5e8feafe"),
                (503063851, "b151b6974d55b800", "8da957432e4d70b8f25eda635216e2d4"),
                (1263025280, "6b2a859227eb5296", "f0e0842fb3db47dfe623cb7e755ac89e"),
                (3682418166, "343a6a19bbe5d57c", "4be093ea85965b1a890911690d971ece"),
                (3797392279, "21246753cc8a6cb5", "7e8930928ae85fe3e8e99c0aaa4f02e3"),
                (583582605, "f12a9c547ecba8cc", "b0de5a0afe51d3152c9082370063a71a"),
                (1186855927, "b07fdc17000c5316", "c09dcf91e03275d6dbdde314bd4a2d83"),
                (964173659, "839e7ed97b2d37e7", "61ccf7e061d5d0816fd64286f3a5b4ed"),
                (1178665347, "69ca2bec0b1b7f8b", "bd18805bcc39f82feae5fb887b15d8de"),
                (2576144544, "5fd94e7dc46d19b2", "8ba484985357d5281e5f99380f92832e"),
                (1569833173, "90216c54721780d3", "712bdd54f281f8a4573ee82b0e88c24b"),
                (267072739, "37f1471216e8fc00", "21541b84d720a24bdc75689925a24652"),
                (3331701537, "1daea0de54bb5f47", "3a283b45b4e6f7789c5f3cd63c937d09"),
                (2017911385, "41e2136e8bc642ca", "84494f106a52405eb85b0a732e529987"),
            };

        public static IReadOnlyList<(uint seed, uint mulStep, string baseStep, uint[] expected)> ParameterizedJumpSample =
            new (uint seed, uint mulStep, string baseStep, uint[] expected)[]

            {
                (42, 4, "0x40000000", new[] {1487782329u, 2431386006u, 46422321u, 1262793750u,}),
                (42, 1073741824, "4", new[] {1487782329u, 2431386006u, 46422321u, 1262793750u,}),
                (114514, 4294967295, "100000000", new[] {1989539249u, 167743719u, 2445625427u, 1312310908u,}),
                (114514, 4294967295, "0x100000000", new[] {1989539249u, 167743719u, 2445625427u, 1312310908u,})
            };

        private readonly ITestOutputHelper _output;

        public JumpTest(ITestOutputHelper output) => _output = output;

        [Fact]
        public void CalcTest()
        {
            static void assert(ReadOnlySpan<char> actual, ReadOnlySpan<char> expected)
            {
                var a = new string(actual).Trim('\0');
                var b = new string(expected);

                a.Is(b);
            }

            Span<char> buff = stackalloc char[33];

            foreach (var sample in TestSamples)
            {
                XorShiftAddCore.CalculateJumpPolynomial(buff, sample.mulStep, sample.baseStep);
                assert(buff, sample.expected);
            }
        }

        [Fact]
        public void ParameterizedJumpTest()
        {
            static unsafe void assert(ReadOnlySpan<uint> expected, in XorShiftAddState actual)
            {
                expected.Length.Is(4);

                for (int i = 0; i < expected.Length; i++)
                {
                    expected[i].Is(actual.Vector[i]);
                }
            }

            var actual = new XorShiftAddState();

            foreach (var elem in ParameterizedJumpSample)
            {
                XorShiftAddCore.Init(ref actual, elem.seed);
                XorShiftAddCore.Jump(ref actual, elem.mulStep, elem.baseStep);

                assert(elem.expected, actual);
            }
        }



    }
}
