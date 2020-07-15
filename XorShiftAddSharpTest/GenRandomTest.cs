using System;
using System.Collections.Generic;
using ChainingAssertion;
using XorShiftAddSharp;
using Xunit;
using Xunit.Abstractions;

namespace XorShiftAddSharpTest
{
    public class GenRandomTest
    {
        private static readonly IReadOnlyList<(uint seed, uint[] samples)> Uint32Sample =
            new (uint seed, uint[] samples)[]
            {
                (4u,
                    new[]
                    {
                        3002512912u, 3855211787u, 3009603880u, 84014133u, 4151800844u, 615010113u, 1953821076u,
                        3708287868u, 2459021233u, 1447158272u,
                    }),
                (93u,
                    new[]
                    {
                        3874896979u, 992977121u, 1069329498u, 3015171156u, 375348349u, 3790976597u, 2841333151u,
                        2276446883u, 970000228u, 845764577u,
                    }),
                (963u,
                    new[]
                    {
                        37117485u, 352567355u, 323699449u, 2848791438u, 2276703781u, 2264072442u, 3443150261u,
                        932743796u, 879617070u, 2035171616u,
                    }),
                (5029u,
                    new[]
                    {
                        207040210u, 3655206612u, 3384915346u, 82458777u, 3881746965u, 1293684085u, 2122537384u,
                        3219258900u, 3111324708u, 1970803679u,
                    }),
                (37729u,
                    new[]
                    {
                        880485157u, 3709302892u, 4030160873u, 2956042216u, 4125812203u, 1070772692u, 3399587874u,
                        593281071u, 1860114651u, 2625519897u,
                    }),
                (826819u,
                    new[]
                    {
                        3664012915u, 958770258u, 1529365168u, 4214564180u, 1242054072u, 4009957510u, 3216644847u,
                        956576561u, 2370761997u, 464126375u,
                    }),
                (4951747u,
                    new[]
                    {
                        3131176615u, 2929519723u, 4133167938u, 2140606778u, 1904061146u, 4076712615u, 2678539291u,
                        3385001297u, 3987652638u, 3461320891u,
                    }),
                (48751289u,
                    new[]
                    {
                        2634979224u, 4019034153u, 3720182122u, 3055821017u, 4232471752u, 3776815359u, 143685564u,
                        4118096195u, 1730809671u, 3243802199u,
                    }),
                (988753232u,
                    new[]
                    {
                        3017359090u, 2481554837u, 2744798895u, 2862006562u, 3929678858u, 3226233293u, 2678561322u,
                        1247285422u, 390816232u, 3749186275u,
                    }),
                (1648654016u,
                    new[]
                    {
                        1411880009u, 2772197364u, 1793272083u, 1863962983u, 2556955074u, 1929066300u, 627062825u,
                        695795133u, 688219904u, 3864739124u,
                    }),
            };

        private static readonly IReadOnlyList<(uint seed, double[] samples)> DoubleSamples =
            new (uint seed, double[] samples)[]
            {

                (2u,
                    new[]
                    {
                        0.8009227791923732, 0.68827257073171144, 0.19759308803079201, 0.80708743050258258,
                        0.31575295772809031, 0.6678826014610556, 0.37425777964454632, 0.0061471174138394424,
                        0.86653808480020755, 0.35294919111679179,
                    }),
                (13u,
                    new[]
                    {
                        0.37690202611142387, 0.43585216393113191, 0.44835315386363495, 0.34966564392850363,
                        0.5969757944414722, 0.090593839107926177, 0.40098446357735484, 0.26899913091302552,
                        0.89490725535061877, 0.67345895660819932,
                    }),
                (229u,
                    new[]
                    {
                        0.81556466253654381, 0.53393344837866941, 0.64943012679778989, 0.33053213680695903,
                        0.82686753464512108, 0.18066892582885408, 0.64378886841745164, 0.18578545530714907,
                        0.69654847303670264, 0.21951948922033471,
                    }),
                (7291u,
                    new[]
                    {
                        0.20818435606240548, 0.2743800785643985, 0.020108148942109549, 0.25933576677737391,
                        0.41342825160453756, 0.75402583437166648, 0.00034478657164593862, 0.31162966037895579,
                        0.93461594520996338, 0.0767763074567821,
                    }),
                (99001u,
                    new[]
                    {
                        0.96780024619651295, 0.74355723498415904, 0.089603416181731288, 0.53233227570288888,
                        0.21204174312667323, 0.57426984285981841, 0.71569425479177695, 0.40418928428425505,
                        0.68157032250939831, 0.15422954497098273,
                    }),
                (522026u,
                    new[]
                    {
                        0.48103809409821907, 0.38951684707362355, 0.11230605587405373, 0.25732449448213102,
                        0.68075294042765266, 0.46873837797608497, 0.77691274352542306, 0.30062026432524958,
                        0.19636382742368985, 0.92752921737417593,
                    }),
                (3943601u,
                    new[]
                    {
                        0.91609819782102719, 0.20829030293719208, 0.24671292016792423, 0.80731396563076019,
                        0.13758147615005178, 0.83510155882614989, 0.86755609992669158, 0.13219648923900307,
                        0.43427810460921734, 0.28824266623793671,
                    }),
                (80513882u,
                    new[]
                    {
                        0.42031020735986058, 0.26285035576741744, 0.85631403386996419, 0.98602426177386304,
                        0.01097303518513848, 0.63539992225816699, 0.22238889444514731, 0.92134917876309141,
                        0.69378209487260589, 0.97714162324164278,
                    }),
                (811750161u,
                    new[]
                    {
                        0.69464733385442268, 0.14044281592224295, 0.5140481597515929, 0.11645331511353574,
                        0.87684160349676121, 0.26445348960065151, 0.25120314914013198, 0.60087809387504543,
                        0.081022128647651681, 0.26544796323690478,
                    }),
                (3924275839u,
                    new[]
                    {
                        0.29436174774779544, 0.19612181905360926, 0.80376751458728135, 0.96452556051615124,
                        0.32350730145473305, 0.82156066720369614, 0.84472519715633809, 0.54882291192037136,
                        0.11116041301703561, 0.56506023750496592,
                    }),

            };

        private static readonly IReadOnlyList<(uint seed, float[] expected)> FloatSamples =
            new (uint seed, float[] expected)[]
            {
                (0u,
                    new[]
                    {
                        0.147181153f, 0.285405099f, 0.630019248f, 0.369910479f, 0.636221051f, 0.636961043f,
                        0.557494223f, 0.81858927f, 0.828093171f, 0.236378372f,
                    }),
                (75u,
                    new[]
                    {
                        0.148638546f, 0.273290038f, 0.791864574f, 0.0387658477f, 0.0952485204f, 0.464929402f,
                        0.536182046f, 0.115171432f, 0.842569947f, 0.213809788f,
                    }),
                (482u,
                    new[]
                    {
                        0.952811956f, 0.892531097f, 0.250929713f, 0.297591686f, 0.697857738f, 0.653637409f,
                        0.485988796f, 0.416533053f, 0.563969135f, 0.745387435f,
                    }),
                (1382u,
                    new[]
                    {
                        0.467585862f, 0.66499418f, 0.578468621f, 0.578629732f, 0.387828708f, 0.392836988f, 0.435968101f,
                        0.792633712f, 0.921586692f, 0.592482269f,
                    }),
                (52435u,
                    new[]
                    {
                        0.354433537f, 0.626465917f, 0.828648925f, 0.654732943f, 0.674548507f, 0.413722873f,
                        0.131066561f, 0.168216646f, 0.252923369f, 0.163761735f,
                    }),
                (918800u,
                    new[]
                    {
                        0.938631773f, 0.13845396f, 0.615034401f, 0.191283226f, 0.841174722f, 0.762147844f,
                        0.0535657406f, 0.420780063f, 0.513176918f, 0.590735197f,
                    }),
                (9655022u,
                    new[]
                    {
                        0.512489378f, 0.927072048f, 0.431889832f, 0.259863317f, 0.00644278526f, 0.316207349f,
                        0.288168013f, 0.166806996f, 0.510797203f, 0.900176764f,
                    }),
                (97559282u,
                    new[]
                    {
                        0.484379351f, 0.342631757f, 0.151389241f, 0.972900748f, 0.794551253f, 0.440330744f,
                        0.201979935f, 0.602143407f, 0.877998948f, 0.629392862f,
                    }),
                (826312465u,
                    new[]
                    {
                        0.280151606f, 0.42151612f, 0.154342115f, 0.643180728f, 0.316951692f, 0.0482774377f,
                        0.487761557f, 0.801322579f, 0.200073898f, 0.739159942f,
                    }),
                (3689121869u,
                    new[]
                    {
                        0.652815819f, 0.619573772f, 0.599508405f, 0.486468315f, 0.177405775f, 0.735032082f,
                        0.236526668f, 0.0557280779f, 0.816220045f, 0.302786589f,
                    }),
            };

        private static readonly IReadOnlyList<(uint seed, float[] expected)> Float0Samples =
            new (uint seed, float[] expected)[]
            {
                (5u,
                    new[]
                    {
                        0.677891493f, 0.482639372f, 0.529338002f, 0.792118073f, 0.303466678f, 0.526518881f,
                        0.0615257025f, 0.236647844f, 0.51324892f, 0.417109132f,
                    }),
                (64u,
                    new[]
                    {
                        0.680356741f, 0.443345666f, 0.929308653f, 0.921043336f, 0.0348529816f, 0.954088628f,
                        0.422628522f, 0.0188707709f, 0.960977435f, 0.780547738f,
                    }),
                (265u,
                    new[]
                    {
                        0.00385403633f, 0.281495452f, 0.742548048f, 0.52800113f, 0.247458339f, 0.514229178f,
                        0.0230588317f, 0.669995666f, 0.982246995f, 0.761617124f,
                    }),
                (8881u,
                    new[]
                    {
                        0.662068725f, 0.307940722f, 0.134866476f, 0.0684145689f, 0.948963583f, 0.693920553f,
                        0.32690233f, 0.952808678f, 0.71383369f, 0.462949932f,
                    }),
                (53765u,
                    new[]
                    {
                        0.396522462f, 0.677662551f, 0.211898327f, 0.563638866f, 0.136944294f, 0.515254915f,
                        0.0298690796f, 0.154812396f, 0.130402207f, 0.196701288f,
                    }),
                (572149u,
                    new[]
                    {
                        0.331526399f, 0.806454122f, 0.141674638f, 0.0883294344f, 0.652065575f, 0.623422563f,
                        0.913734794f, 0.493138373f, 0.761253655f, 0.278327525f,
                    }),
                (1122012u,
                    new[]
                    {
                        0.675249696f, 0.831850529f, 0.111075222f, 0.718478203f, 0.242535114f, 0.0704522729f,
                        0.013964951f, 0.349045277f, 0.999836743f, 0.496868849f,
                    }),
                (30370310u,
                    new[]
                    {
                        0.76175195f, 0.0241473317f, 0.0712578297f, 0.0320695043f, 0.10756731f, 0.0943101048f,
                        0.874464452f, 0.1077196f, 0.293616056f, 0.505338252f,
                    }),
                (812156690u,
                    new[]
                    {
                        0.681477785f, 0.581609428f, 0.400152147f, 0.0415619016f, 0.862211764f, 0.214655995f,
                        0.650927782f, 0.852764189f, 0.287271321f, 0.866290212f,
                    }),
                (4059636650u,
                    new[]
                    {
                        0.881676435f, 0.269600391f, 0.229118288f, 0.31242913f, 0.703311861f, 0.450122714f, 0.176206827f,
                        0.0472293496f, 0.217641056f, 0.219241381f,
                    }),
            };

        private readonly ITestOutputHelper _output;
        public GenRandomTest(ITestOutputHelper output) => _output = output;


        [Fact]
        public void Uint32Test()
        {
            static void assert(uint seed, ReadOnlySpan<uint> expected)
            {
                var state = new uint[4];
                XsAddCore.Init(state, seed);

                foreach (var elem in expected)
                {
                    XsAddCore.NextUint32(state).Is(elem);
                }
            }

            foreach (var elem in Uint32Sample)
            {
                assert(elem.seed, elem.samples);
            }
        }

        [Fact]
        public void DoubleTest()
        {
            static void assert(uint seed, ReadOnlySpan<double> expected)
            {
                var state = new uint[4];
                XsAddCore.Init(state, seed);

                foreach (var elem in expected)
                {
                    XsAddCore.NextDouble(state).Is(elem);
                }
            }

            foreach (var elem in DoubleSamples)
            {
                assert(elem.seed, elem.samples);
            }
        }

        [Fact]
        public void FloatTest()
        {
            static void assert(uint seed, ReadOnlySpan<float> expected)
            {
                var state = new uint[4];
                XsAddCore.Init(state, seed);

                foreach (var elem in expected)
                {
                    XsAddCore.NextFloat(state).Is(elem);
                }
            }

            foreach (var elem in FloatSamples)
            {
                assert(elem.seed, elem.expected);
            }
        }

        [Fact]
        public void Float0Test()
        {
            static void assert(uint seed, ReadOnlySpan<float> expected)
            {
                var state = new uint[4];
                XsAddCore.Init(state, seed);

                foreach (var elem in expected)
                {
                    XsAddCore.XsAddFloatOC(state).Is(elem);
                }
            }

            foreach (var elem in Float0Samples)
            {
                assert(elem.seed, elem.expected);
            }
        }


    }


}
