using System;
using System.Collections.Generic;
using ChainingAssertion;
using XorShiftAddSharp;
using Xunit;
using Xunit.Abstractions;

namespace XorShiftAddSharpTest
{
    public class InitTest
    {
        private static readonly IReadOnlyList<(uint seed, uint[] expectedSamples)> ScalarInit =
            new (uint seed, uint[] expectedSamples)[]
            {
                (8u, new[] {640872880u, 1001586298u, 3078648866u, 266899571u,}),
                (83u, new[] {3153496885u, 2657940665u, 1147456779u, 3044374165u,}),
                (757u, new[] {3274515979u, 2287658052u, 3750665483u, 3497756563u,}),
                (4559u, new[] {150053804u, 2486411914u, 3576275757u, 3176829266u,}),
                (80573u, new[] {236770142u, 1763584009u, 902453743u, 187680774u,}),
                (745769u, new[] {1148241769u, 1660294930u, 1556737897u, 3534977803u,}),
                (7240430u, new[] {346063871u, 1993421808u, 468136289u, 3912612592u,}),
                (62962334u, new[] {3705098116u, 2807073256u, 2624716792u, 672009809u,}),
                (863352607u, new[] {548435437u, 934286423u, 2258472427u, 720932129u,}),
                (1964401259u, new[] {1182503146u, 1320586551u, 865051370u, 843308107u,}),
            };

        private static readonly IReadOnlyList<(uint[] input, uint[] expectedState)> ArrayInit =
            new (uint[] input, uint[] expectedState)[]
            {
                (new[] {5u,}, new[] {1466200539u, 3317907333u, 1584951589u, 1734481336u,}),
                (new[] {5u, 17u,}, new[] {3412885069u, 1653572037u, 2454701209u, 3749003219u,}),
                (new[] {9u, 54u, 733u,}, new[] {1348408157u, 3779989240u, 4118328105u, 2694199378u,}),
                (new[] {9u, 75u, 578u, 9515u,}, new[] {3187778775u, 2430683583u, 1709816018u, 3516144775u,}),
                (new[] {9u, 79u, 502u, 7063u, 92479u,}, new[] {277440214u, 3882046275u, 1756504178u, 2531075522u,}),
                (new[] {5u, 27u, 859u, 8521u, 65923u, 994208u,},
                    new[] {3127544795u, 736424726u, 476822012u, 978532559u,}),
                (new[] {0u, 85u, 785u, 9809u, 93773u, 451161u, 4951637u,},
                    new[] {3590972222u, 3520363610u, 4109729293u, 733286531u,}),
                (new[] {1u, 77u, 377u, 8400u, 86185u, 840825u, 3967799u, 86218642u,},
                    new[] {1771786663u, 363726855u, 2411801632u, 244340518u,}),
                (new[] {4u, 23u, 662u, 1426u, 27161u, 683189u, 2385747u, 19569748u, 813685783u,},
                    new[] {4119520269u, 2246263334u, 872454083u, 1685043549u,}),
                (new[] {7u, 89u, 907u, 6578u, 85629u, 491731u, 2190420u, 79388487u, 497983710u, 2345026250u,},
                    new[] {2708475998u, 3518787455u, 551937418u, 3888595963u,}),

            };

        private readonly ITestOutputHelper _output;

        public InitTest(ITestOutputHelper output) => _output = output;

        static void Assert(ReadOnlySpan<uint> actual, ReadOnlySpan<uint> expected)
        {
            actual.Length.Is(4);

            for (int i = 0; i < 4; i++)
            {
                actual[i].Is(expected[i]);
            }
        }


        [Fact]
        public void InitUit32Test()
        {
            uint[] actual = new uint[4];


            foreach (var elem in ScalarInit)
            {
                XSAddCore.xsadd_init(actual, elem.seed);
                Assert(actual, elem.expectedSamples);
            }
        }

        [Fact]
        public void InitArrayTest()
        {
            uint[] actual = new uint[4];

            foreach (var elem in ArrayInit)
            {
                XSAddCore.xsadd_init_by_array(actual, elem.input, elem.input.Length);

                Assert(actual, elem.expectedState);
            }
        }

    }
}
