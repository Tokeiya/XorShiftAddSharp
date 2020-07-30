using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace XorShiftAddSharp
{
    /// <summary>
    ///     Naive implementation of XORSHIFT-ADD
    ///     See the original doxygen for details.
    /// </summary>
    public static unsafe class XorShiftAddCore
    {
        private const float XsaddFloatMul = 1.0f / 16777216.0f;
        private const double XsaddDoubleMul = 1.0 / 9007199254740992.0;

        private const int Loop = 8;
        private const int PolynomialArraySize = 8;
        private const int UzArraySize = 8;

        public const int JumpStrSize = 33;


        private const string CharacteristicPolynomial = "100000000008101840085118000000001";

        private const string XsaddJumpBaseStep = "1FA2A1CF67B5FB863";

        public static void NextState(ref InternalState xsadd)
        {
            const int sh1 = 15;
            const int sh2 = 18;
            const int sh3 = 11;

            uint t;
            t = xsadd.State[0];
            t ^= t << sh1;
            t ^= t >> sh2;
            t ^= xsadd.State[3] << sh3;
            xsadd.State[0] = xsadd.State[1];
            xsadd.State[1] = xsadd.State[2];
            xsadd.State[2] = xsadd.State[3];
            xsadd.State[3] = t;
        }

        public static uint NextUint32(ref InternalState xsadd)
        {
            NextState(ref xsadd);
            return xsadd.State[3] + xsadd.State[2];
        }

        public static float NextFloat(ref InternalState xsadd)
        {
            return (NextUint32(ref xsadd) >> 8) * XsaddFloatMul;
        }


        // ReSharper disable once InconsistentNaming
        public static float XsAddFloatOC(ref InternalState xsadd)
        {
            NextState(ref xsadd);
            return 1.0f - NextFloat(ref xsadd);
        }

        public static double NextDouble(ref InternalState xsadd)
        {
            ulong a = NextUint32(ref xsadd);
            ulong b = NextUint32(ref xsadd);
            a = (a << 21) | (b >> 11);
            return a * XsaddDoubleMul;
        }

        public static void Init(out InternalState xsadd, uint seed)
        {
            xsadd.State[0] = seed;
            xsadd.State[1] = 0;
            xsadd.State[2] = 0;
            xsadd.State[3] = 0;

            for (uint i = 1; i < Loop; i++)
                xsadd.State[(int) i & 3] ^= i + 1812433253u *
                    (xsadd.State[(int) (i - 1) & 3] ^
                     (xsadd.State[(int) (i - 1) & 3] >> 30));

            PeriodCertification(ref xsadd);
            for (int i = 0; i < Loop; i++) NextState(ref xsadd);
        }

        public static void Init(out InternalState xsadd, ReadOnlySpan<uint> initKey)
        {
            const int lag = 1;
            const int mid = 1;
            const int size = 4;
            int i, j;
            uint count;

            xsadd.State[0] = 0;
            xsadd.State[1] = 0;
            xsadd.State[2] = 0;
            xsadd.State[3] = 0;
            if (initKey.Length + 1 > Loop)
                count = (uint) initKey.Length + 1;
            else
                count = Loop;

            var r = IniFunc1(xsadd.State[0] ^ xsadd.State[mid % size]
                                             ^ xsadd.State[(size - 1) % size]);
            xsadd.State[mid % size] += r;
            r += (uint) initKey.Length;
            xsadd.State[(mid + lag) % size] += r;
            xsadd.State[0] = r;
            count--;
            for (i = 1, j = 0; j < count && j < initKey.Length; j++)
            {
                r = IniFunc1(xsadd.State[i % size]
                             ^ xsadd.State[(i + mid) % size]
                             ^ xsadd.State[(i + size - 1) % size]);
                xsadd.State[(i + mid) % size] += r;
                r += initKey[j] + (uint) i;
                xsadd.State[(i + mid + lag) % size] += r;
                xsadd.State[i % size] = r;
                i = (i + 1) % size;
            }

            for (; j < count; j++)
            {
                r = IniFunc1(xsadd.State[i % size]
                             ^ xsadd.State[(i + mid) % size]
                             ^ xsadd.State[(i + size - 1) % size]);
                xsadd.State[(i + mid) % size] += r;
                r += (uint) i;
                xsadd.State[(i + mid + lag) % size] += r;
                xsadd.State[i % size] = r;
                i = (i + 1) % size;
            }

            for (j = 0; j < size; j++)
            {
                r = IniFunc2(xsadd.State[i % size]
                             + xsadd.State[(i + mid) % size]
                             + xsadd.State[(i + size - 1) % size]);
                xsadd.State[(i + mid) % size] ^= r;
                r -= (uint) i;
                xsadd.State[(i + mid + lag) % size] ^= r;
                xsadd.State[i % size] = r;
                i = (i + 1) % size;
            }

            PeriodCertification(ref xsadd);
            for (i = 0; i < Loop; i++) NextState(ref xsadd);
        }

        public static void Jump(ref InternalState xsadd, uint mulStep, string baseStep)
        {
            Span<char> jumpStr = stackalloc char[33];
            CalculateJumpPolynomial(jumpStr, mulStep, baseStep);
            Jump(ref xsadd, jumpStr);
        }

        public static void Jump(ref InternalState xsadd, ReadOnlySpan<char> jumpStr)
        {
	        static bool chk(ReadOnlySpan<char> value)
	        {
		        var ret = true;
		        ret &= value[0] == '0';
		        ret &= (value[1] == 'x' || value[1] == 'X');

		        return ret;
	        }

            var tmpA=new string(jumpStr);

	        Span<uint> jumpPoly = stackalloc uint[PolynomialArraySize];

            var tmp = new InternalState();
            ref InternalState work = ref tmp;

            for (var i = 0; i < InternalState.Size; i++) work.State[i] = 0;

            var str = chk(jumpStr) ? jumpStr[2..] : jumpStr;

            var tmpB = new string(str);

            StrToPolynomial(jumpPoly, str);

            for (int i = 0; i < PolynomialArraySize; ++i)
            for (int j = 0; j < 32; ++j)
            {
                uint mask = 1u << j;

                if ((jumpPoly[i] & mask) != 0) xsadd_add(ref work, xsadd);

                NextState(ref xsadd);
            }

            for (var i = 0; i < InternalState.Size; ++i) xsadd.State[i] = work.State[i];
        }


        //char[] jump_star is must.
        public static void CalculateJumpPolynomial(Span<char> jumpStr,
            uint mulStep, string baseStep)
        {
            for (var i = 0; i < jumpStr.Length; ++i) jumpStr[i] = '\0';

            Span<uint> jumpPoly = stackalloc uint[PolynomialArraySize];
            Span<uint> charcteristic = stackalloc uint[PolynomialArraySize];
            Span<uint> tee = stackalloc uint[PolynomialArraySize];

            Span<ushort> @base = stackalloc ushort[UzArraySize];
            Span<ushort> mul = stackalloc ushort[UzArraySize];
            Span<ushort> step = stackalloc ushort[UzArraySize];

            StrToPolynomial(charcteristic, CharacteristicPolynomial);
            Clear(tee);

            tee[0] = 2;

            String16ToUz(@base, baseStep.ToLower().Replace("0x", ""));
            Uint32ToUz(mul, mulStep);
            UzMul(step, mul, @base);
            PolynomialPowerMod(jumpPoly, tee, step, charcteristic);
            PolynomialToStr(jumpStr, jumpPoly);
        }

        private static void xsadd_add(ref InternalState dest, in InternalState src)
        {
            dest.State[0] ^= src.State[0];
            dest.State[1] ^= src.State[1];
            dest.State[2] ^= src.State[2];
            dest.State[3] ^= src.State[3];
        }

        private static void PeriodCertification(ref InternalState xsadd)
        {
            if (xsadd.State[0] == 0 &&
                xsadd.State[1] == 0 &&
                xsadd.State[2] == 0 &&
                xsadd.State[3] == 0)
            {
                xsadd.State[0] = 'X';
                xsadd.State[1] = 'S';
                xsadd.State[2] = 'A';
                xsadd.State[3] = 'D';
            }
        }

        private static void Add(Span<uint> dest, ReadOnlySpan<uint> src)
        {
            for (int i = 0; i < PolynomialArraySize; i++) dest[i] ^= src[i];
        }

        private static void Clear(Span<uint> dest)
        {
            for (int i = 0; i < PolynomialArraySize; i++) dest[i] = 0;
        }

        private static void ShiftUp1(Span<uint> dest)
        {
            uint lsb = 0;

            for (int i = 0; i < PolynomialArraySize; i++)
            {
                uint tmp = dest[i] >> 31;
                dest[i] = (dest[i] << 1) | lsb;
                lsb = tmp;
            }
        }

        // ReSharper disable once InconsistentNaming
        private static void ShiftUp0n(Span<uint> dest, int n)
        {
            uint lsb = 0;
            for (int i = 0; i < PolynomialArraySize; i++)
            {
                uint tmp = dest[i] >> (32 - n);
                dest[i] = (dest[i] << n) | lsb;
                lsb = tmp;
            }
        }

        private static void ShiftDown1(Span<uint> dest)
        {
            uint msb = 0;
            for (int i = PolynomialArraySize - 1; i >= 0; i--)
            {
                uint tmp = dest[i] << 31;
                dest[i] = (dest[i] >> 1) | msb;
                msb = tmp;
            }
        }

        private static void ShiftUp(Span<uint> dest, int n)
        {
            int q = n / 32;
            int r = n % 32;
            if (q == 0)
            {
                ShiftUp0n(dest, r);
                return;
            }

            if (q >= PolynomialArraySize)
            {
                Clear(dest);
                return;
            }

            for (int i = PolynomialArraySize - 1; i >= q; i--)
            {
                uint lower;
                if (r != 0 && i - q - 1 >= 0)
                    lower = dest[i - q - 1] >> (32 - r);
                else
                    lower = 0;

                dest[i] = (dest[i - q] << r) | lower;
            }

            for (int i = q - 1; i >= 0; i--) dest[i] = 0;
        }

        private static int Deg(ReadOnlySpan<uint> x)
        {
            return DegLazy(x, PolynomialArraySize * 32 - 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int DegLazy(ReadOnlySpan<uint> x, int preDeg)
        {
            int deg = preDeg;
            uint mask;
            int index = preDeg / 32;
            int bitPos = preDeg % 32;
            if (index >= PolynomialArraySize)
            {
                index = PolynomialArraySize - 1;
                bitPos = 31;
                deg = (index + 1) * 32 - 1;
            }

            mask = 1u << bitPos;
            for (int i = index; i >= 0; i--)
            {
                while (mask != 0)
                {
                    if ((x[i] & mask) != 0) return deg;

                    mask = mask >> 1;
                    deg--;
                }

                mask = (uint) (1LU << 31);
            }

            return -1;
        }

        private static uint IniFunc1(uint x)
        {
            return (x ^ (x >> 27)) * 1664525u;
        }

        private static uint IniFunc2(uint x)
        {
            return (x ^ (x >> 27)) * 1566083941u;
        }

        private static void Mul(Span<uint> x, ReadOnlySpan<uint> y)
        {
            Span<uint> result = stackalloc uint[PolynomialArraySize];
            Clear(result);

            for (int i = 0; i < PolynomialArraySize; i++)
            {
                uint u = y[i];
                for (int j = 0; j < 32; j++)
                {
                    if ((u & 1) != 0) Add(result, x);

                    ShiftUp1(x);
                    u = u >> 1;
                }
            }

            result.CopyTo(x);
        }

        private static void Square(Span<uint> x)
        {
            Span<uint> tmp = stackalloc uint[PolynomialArraySize];
            x.CopyTo(tmp);
            Mul(x, tmp);
        }

        private static void Mod(Span<uint> dest, ReadOnlySpan<uint> x)
        {
            Span<uint> tmp = stackalloc uint[PolynomialArraySize];

            int degree = Deg(x);
            int destDeg = Deg(dest);
            int diff = destDeg - degree;
            int tmpDeg = degree;
            if (diff < 0) return;

            //Check!
            x.CopyTo(tmp);

            if (diff == 0)
            {
                Add(dest, tmp);
                return;
            }

            ShiftUp(tmp, diff);
            tmpDeg += diff;
            Add(dest, tmp);
            destDeg = DegLazy(dest, destDeg);
            while (destDeg >= degree)
            {
                ShiftDown1(tmp);
                tmpDeg--;
                if (destDeg == tmpDeg)
                {
                    Add(dest, tmp);
                    destDeg = DegLazy(dest, destDeg);
                }
            }
        }

        private static int StrLen(ReadOnlySpan<char> scr)
        {
            for (int i = 0; i < scr.Length; i++)
                if (scr[i] == '\0')
                    return i;

            return scr.Length;
        }

        // ReSharper disable once InconsistentNaming
        private static uint StrToUL(ReadOnlySpan<char> scr, int idx)
        {
            var end = 0;

            for (; end < scr.Length; ++end)
                if (scr[end] == '\0')
                    break;


            return uint.Parse(scr[idx..end], NumberStyles.HexNumber);
        }

        private static void SprintF(Span<char> buffer, int idx, string format, uint value)
        {
            var tmp = value.ToString(format);

            var piv = idx;

            foreach (var elem in tmp) buffer[piv++] = elem;
        }

        private static void StrToPolynomial(Span<uint> poly, ReadOnlySpan<char> str)
        {
            Span<char> buffer = stackalloc char[PolynomialArraySize * 8 + 1];
            str.CopyTo(buffer);

            buffer[PolynomialArraySize * 8] = '\0';

            int len = StrLen(buffer);

            int pos = len - 8;
            int i;
            for (i = 0; pos >= 0 && i < PolynomialArraySize; i++)
            {
                poly[i] = StrToUL(buffer, pos);
                buffer[pos] = (char) 0;
                pos -= 8;
            }

            if (pos + 8 > 0)
            {
                poly[i] = StrToUL(buffer, 0);
                i++;
            }

            for (; i < PolynomialArraySize; i++) poly[i] = 0;
        }

        private static void PolynomialToStr(Span<char> str, Span<uint> poly)
        {
            int first = 1;
            int pos = 0;
            for (int i = PolynomialArraySize - 1; i >= 0; i--)
                if (first != 0)
                {
                    if (poly[i] != 0)
                    {
                        SprintF(str, pos, "x", poly[i]);
                        pos = StrLen(str);
                        first = 0;
                    }
                }
                else
                {
                    SprintF(str, pos, "x08", poly[i]);
                    pos = StrLen(str);
                }

            if (first != 0)
                for (var i = 0; i < str.Length; ++i)
                    str[i] = '0';
        }

        private static void PolynomialPowerMod(Span<uint> dest,
            ReadOnlySpan<uint> x,
            Span<ushort> power,
            ReadOnlySpan<uint> modPoly)
        {
            Span<uint> tmp = stackalloc uint[PolynomialArraySize];
            Span<uint> result = stackalloc uint[PolynomialArraySize];

            x.CopyTo(tmp);

            Clear(result);
            result[0] = 1;

            for (int i = 0; i < PolynomialArraySize; i++)
            {
                ushort tmpPower = power[i];
                for (int j = 0; j < 16; j++)
                {
                    if ((tmpPower & 1) != 0)
                    {
                        Mul(result, tmp);
                        Mod(result, modPoly);
                    }

                    Square(tmp);
                    Mod(tmp, modPoly);
                    tmpPower = (ushort) (tmpPower >> 1);
                }
            }

            result.CopyTo(dest);
        }

        private static void UzClear(Span<ushort> a)
        {
            for (int i = 0; i < UzArraySize; i++) a[i] = 0;
        }

        private static void Uint32ToUz(Span<ushort> x, uint y)
        {
            UzClear(x);
            x[0] = (ushort) (y & 0xffff);
            x[1] = (ushort) (y >> 16);
        }

        private static void UzMul(Span<ushort> result, ReadOnlySpan<ushort> x, ReadOnlySpan<ushort> y)
        {
            uint tmp;
            const uint lMask = 0xffffU;

            UzClear(result);
            for (int i = 0; i < UzArraySize; i++)
            for (int j = 0; j < UzArraySize; j++)
            {
                tmp = (uint) (x[i] * y[j]);
                if (i + j >= UzArraySize) break;
                tmp += result[i + j];
                result[i + j] = (ushort) (tmp & lMask);
                for (int k = i + j + 1; k < UzArraySize; k++)
                {
                    tmp = tmp >> 16;
                    if (tmp == 0) break;
                    tmp += result[k];
                    result[k] = (ushort) (tmp & lMask);
                }
            }
        }

        private static void String16ToUz(Span<ushort> result, ReadOnlySpan<char> str)
        {
            Span<char> s = stackalloc char[4 * UzArraySize + 1];

            for (var i = 0; i < s.Length; ++i) s[i] = (char) 0;

            //strncpy(s, str, 4 * UZ_ARRAY_SIZE);

            for (int i = 0; i < str.Length; i++) s[i] = str[i];


            s[4 * UzArraySize] = '\0';
            int len = StrLen(s);
            UzClear(result);
            for (int i = 0; i < UzArraySize; i++)
            {
                len = len - 4;
                if (len < 0) len = 0;

                ushort tmp = (ushort) StrToUL(s, len);
                result[i] = tmp;
                if (len == 0) break;
                s[len] = '\0';
            }
        }
    }
}