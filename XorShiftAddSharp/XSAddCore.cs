using System;
using System.Buffers;
using System.ComponentModel.Design;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.Unicode;
using System.Threading.Tasks.Sources;

namespace XorShiftAddSharp
{
    /**
 * Polynomial over F<sub>2</sub>
 * LSB of ar[0], i.e. ar[0] & 1, represent constant
 */


    public static class XSAddCore
    {
        public const float XSADD_FLOAT_MUL = (1.0f / 16777216.0f);
        public const double XSADD_DOUBLE_MUL = (1.0 / 9007199254740992.0);


        public const int LOOP = 8;
        public const int POLYNOMIAL_ARRAY_SIZE = 8;
        public const int UZ_ARRAY_SIZE = 8;

        private static uint[] CreateF2_POLYNOMIAL_T() => new uint[POLYNOMIAL_ARRAY_SIZE];
        private static ushort[] CreateUZ_T() => new ushort[UZ_ARRAY_SIZE];


        /*
         * this is hexadecimal string
         */
        private const string characteristic_polynomial = "100000000008101840085118000000001";

        /* 3^41 > 2^64 and 3^41 < 2^65 */
        private const string xsadd_jump_base_step = "1FA2A1CF67B5FB863";

        public static void xsadd_next_state(uint[] xsadd)
        {
            const int sh1 = 15;
            const int sh2 = 18;
            const int sh3 = 11;

            uint t;
            t = xsadd[0];
            t ^= t << sh1;
            t ^= t >> sh2;
            t ^= xsadd[3] << sh3;
            xsadd[0] = xsadd[1];
            xsadd[1] = xsadd[2];
            xsadd[2] = xsadd[3];
            xsadd[3] = t;
        }

        public static uint xsadd_uint32(uint[] xsadd)
        {
            xsadd_next_state(xsadd);
            return xsadd[3] + xsadd[2];
        }

        public static float xsadd_float(uint[] xsadd)
        {
            return (xsadd_uint32(xsadd) >> 8) * XSADD_FLOAT_MUL;
        }

        public static float xsadd_floatOC(uint[] xsadd)
        {
            xsadd_next_state(xsadd);
            return 1.0f - xsadd_float(xsadd);
        }

        public static double xsadd_double(uint[] xsadd)
        {
            ulong a = xsadd_uint32(xsadd);
            ulong b = xsadd_uint32(xsadd);
            a = (a << 21) | (b >> 11);
            return a * XSADD_DOUBLE_MUL;
        }

        static void xsadd_init(uint[] xsadd, uint seed)
        {
            xsadd[0] = seed;
            xsadd[1] = 0;
            xsadd[2] = 0;
            xsadd[3] = 0;

            for (uint i = 1; i < LOOP; i++)
            {
                xsadd[i & 3] ^= i + 1812433253u * (xsadd[(i - 1) & 3] ^ (xsadd[(i - 1) & 3] >> 30));
            }

            period_certification(xsadd);
            for (int i = 0; i < LOOP; i++)
            {
                xsadd_next_state(xsadd);
            }
        }

        public static void xsadd_init_by_array(uint[] random, uint[] init_key, uint key_length)
        {
            const int lag = 1;
            const int mid = 1;
            const int size = 4;
            uint i, j;
            uint count;
            uint r;
            uint[] st = random;

            st[0] = 0;
            st[1] = 0;
            st[2] = 0;
            st[3] = 0;
            if (key_length + 1 > LOOP)
            {
                count = key_length + 1;
            }
            else
            {
                count = LOOP;
            }

            r = ini_func1(st[0] ^ st[mid % size]
                                ^ st[(size - 1) % size]);
            st[mid % size] += r;
            r += key_length;
            st[(mid + lag) % size] += r;
            st[0] = r;
            count--;
            for (i = 1, j = 0; (j < count) && (j < key_length); j++)
            {
                r = ini_func1(st[i % size]
                              ^ st[(i + mid) % size]
                              ^ st[(i + size - 1) % size]);
                st[(i + mid) % size] += r;
                r += init_key[j] + i;
                st[(i + mid + lag) % size] += r;
                st[i % size] = r;
                i = (i + 1) % size;
            }

            for (; j < count; j++)
            {
                r = ini_func1(st[i % size]
                              ^ st[(i + mid) % size]
                              ^ st[(i + size - 1) % size]);
                st[(i + mid) % size] += r;
                r += i;
                st[(i + mid + lag) % size] += r;
                st[i % size] = r;
                i = (i + 1) % size;
            }

            for (j = 0; j < size; j++)
            {
                r = ini_func2(st[i % size]
                              + st[(i + mid) % size]
                              + st[(i + size - 1) % size]);
                st[(i + mid) % size] ^= r;
                r -= i;
                st[(i + mid + lag) % size] ^= r;
                st[i % size] = r;
                i = (i + 1) % size;
            }

            period_certification(random);
            for (i = 0; i < LOOP; i++)
            {
                xsadd_next_state(random);
            }
        }

        public static void xsadd_jump(uint[] xsadd, uint mul_step, string base_step)
        {
            Span<char> jump_str = stackalloc char[33];
            xsadd_calculate_jump_polynomial(jump_str, mul_step, base_step);
            xsadd_jump_by_polynomial(xsadd, jump_str);
        }

        public static void xsadd_jump_by_polynomial(uint[] xsadd, ReadOnlySpan<char> jump_str)
        {

            Span<uint> jump_poly = stackalloc uint[POLYNOMIAL_ARRAY_SIZE];
            Span<uint> work = stackalloc uint[4];

            for (var i = 0; i < work.Length; i++)
            {
                work[i] = 0;
            }

            strtopolynomial(jump_poly, jump_str);

            for (int i = 0; i < POLYNOMIAL_ARRAY_SIZE; ++i)
            {
                for (int j = 0; j < 32; ++j)
                {
                    uint mask = 1u << j;

                    if ((jump_poly[i] & mask) != 0)
                    {
                        xsadd_add(work, xsadd);
                    }

                    xsadd_next_state(xsadd);
                }
            }

            for (var i = 0; i < work.Length; ++i) xsadd[i] = work[i];
        }


        //char[] jump_star is must.
        public static void xsadd_calculate_jump_polynomial(Span<char> jump_str,
            uint mul_step, string base_step)
        {
            Span<uint> jump_poly = stackalloc uint[POLYNOMIAL_ARRAY_SIZE];
            Span<uint> charcteristic = stackalloc uint[POLYNOMIAL_ARRAY_SIZE];
            Span<uint> tee = stackalloc uint[POLYNOMIAL_ARRAY_SIZE];

            Span<ushort> @base = stackalloc ushort[UZ_ARRAY_SIZE];
            Span<ushort> mul = stackalloc ushort[UZ_ARRAY_SIZE];
            Span<ushort> step = stackalloc ushort[UZ_ARRAY_SIZE];

            strtopolynomial(charcteristic, characteristic_polynomial);
            clear(tee);

            tee[0] = 2;

            string16touz(@base, base_step);
            uint32touz(mul, mul_step);
            uz_mul(step, mul, @base);
            polynomial_power_mod(jump_poly, tee, step, charcteristic);
            polynomialtostr(jump_str, jump_poly);
        }

        static void xsadd_add(Span<uint> dest, Span<uint> src)
        {
            dest[0] ^= src[0];
            dest[1] ^= src[1];
            dest[2] ^= src[2];
            dest[3] ^= src[3];
        }

        static void period_certification(uint[] xsadd)
        {
            if (xsadd[0] == 0 &&
                xsadd[1] == 0 &&
                xsadd[2] == 0 &&
                xsadd[3] == 0)
            {
                xsadd[0] = 'X';
                xsadd[1] = 'S';
                xsadd[2] = 'A';
                xsadd[3] = 'D';
            }
        }

        static void add(Span<uint> dest, ReadOnlySpan<uint> src)
        {
            for (int i = 0; i < POLYNOMIAL_ARRAY_SIZE; i++)
            {
                dest[i] ^= src[i];
            }
        }

        static void clear(Span<uint> dest)
        {
            for (int i = 0; i < POLYNOMIAL_ARRAY_SIZE; i++)
            {
                dest[i] = 0;
            }
        }

        static void shiftup1(Span<uint> dest)
        {
            uint lsb = 0;

            for (int i = 0; i < POLYNOMIAL_ARRAY_SIZE; i++)
            {
                uint tmp = dest[i] >> 31;
                dest[i] = (dest[i] << 1) | lsb;
                lsb = tmp;
            }
        }

        static void shiftup0n(Span<uint> dest, int n)
        {
            uint lsb = 0;
            for (int i = 0; i < POLYNOMIAL_ARRAY_SIZE; i++)
            {
                uint tmp = dest[i] >> (32 - n);
                dest[i] = (dest[i] << n) | lsb;
                lsb = tmp;
            }
        }

        static void shiftdown1(Span<uint> dest)
        {
            uint msb = 0;
            for (int i = POLYNOMIAL_ARRAY_SIZE - 1; i >= 0; i--)
            {
                uint tmp = dest[i] << 31;
                dest[i] = (dest[i] >> 1) | msb;
                msb = tmp;
            }
        }

        static void shiftup(Span<uint> dest, int n)
        {
            int q = n / 32;
            int r = n % 32;
            if (q == 0)
            {
                shiftup0n(dest, r);
                return;
            }

            if (q >= POLYNOMIAL_ARRAY_SIZE)
            {
                clear(dest);
                return;
            }

            for (int i = POLYNOMIAL_ARRAY_SIZE - 1; i >= q; i--)
            {
                uint lower;
                if ((r != 0) && (i - q - 1 >= 0))
                {
                    lower = dest[i - q - 1] >> (32 - r);
                }
                else
                {
                    lower = 0;
                }

                dest[i] = dest[i - q] << r | lower;
            }

            for (int i = q - 1; i >= 0; i--)
            {
                dest[i] = 0;
            }
        }

        static int deg(ReadOnlySpan<uint> x)
        {
            return deg_lazy(x, POLYNOMIAL_ARRAY_SIZE * 32 - 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int deg_lazy(ReadOnlySpan<uint> x, int pre_deg)
        {
            int deg = pre_deg;
            uint mask;
            int index = pre_deg / 32;
            int bit_pos = pre_deg % 32;
            if (index >= POLYNOMIAL_ARRAY_SIZE)
            {
                index = POLYNOMIAL_ARRAY_SIZE - 1;
                bit_pos = 31;
                deg = (index + 1) * 32 - 1;
            }

            mask = 1u << bit_pos;
            for (int i = index; i >= 0; i--)
            {
                while (mask != 0)
                {
                    if ((x[i] & mask) != 0)
                    {
                        return deg;
                    }

                    mask = mask >> 1;
                    deg--;
                }

                mask = (uint) (1LU << 31);
            }

            return -1;
        }

        static uint ini_func1(uint x)
        {
            return (x ^ (x >> 27)) * 1664525u;
        }

        static uint ini_func2(uint x)
        {
            return (x ^ (x >> 27)) * 1566083941u;
        }

        static void mul(Span<uint> x, ReadOnlySpan<uint> y)
        {
            Span<uint> result = stackalloc uint[POLYNOMIAL_ARRAY_SIZE];
            clear(result);

            for (int i = 0; i < POLYNOMIAL_ARRAY_SIZE; i++)
            {
                uint u = y[i];
                for (int j = 0; j < 32; j++)
                {
                    if ((u & 1) != 0)
                    {
                        add(result, x);
                    }

                    shiftup1(x);
                    u = u >> 1;
                }
            }
            //*x = *result;

            result.CopyTo(x);
        }

        static void square(Span<uint> x)
        {
            Span<uint> tmp = stackalloc uint[POLYNOMIAL_ARRAY_SIZE];
            x.CopyTo(tmp);
            mul(x, tmp);
        }

        static void mod(Span<uint> dest, ReadOnlySpan<uint> x)
        {

            Span<uint> tmp = stackalloc uint[POLYNOMIAL_ARRAY_SIZE];

            int degree = deg(x);
            int dest_deg = deg(dest);
            int diff = dest_deg - degree;
            int tmp_deg = degree;
            if (diff < 0)
            {
                return;
            }

            //Check!
            x.CopyTo(tmp);

            if (diff == 0)
            {
                add(dest, tmp);
                return;
            }

            shiftup(tmp, diff);
            tmp_deg += diff;
            add(dest, tmp);
            dest_deg = deg_lazy(dest, dest_deg);
            while (dest_deg >= degree)
            {
                shiftdown1(tmp);
                tmp_deg--;
                if (dest_deg == tmp_deg)
                {
                    add(dest, tmp);
                    dest_deg = deg_lazy(dest, dest_deg);
                }
            }
        }

        static int strlen(ReadOnlySpan<char> scr)
        {
            for (int i = 0; i < scr.Length; i++)
            {
                if (scr[i] == '\0') return i;
            }

            return scr.Length;
        }

        static uint strtoul(ReadOnlySpan<char> scr, int idx)
        {
            var end = 0;

            for (; end < scr.Length; ++end)
                if (scr[end] == '\0')
                    break;



            return uint.Parse(scr[idx..end], NumberStyles.HexNumber);
        }

        static void sprintf(Span<char> buffer, int idx,string format, uint value)
        {
            var tmp = value.ToString(format);

            var piv = idx;

            foreach (var elem in tmp)
            {
                buffer[piv++] = elem;
            }
            
        }

        static void strtopolynomial(Span<uint> poly, ReadOnlySpan<char> str)
        {


            Span<char> buffer = stackalloc char[POLYNOMIAL_ARRAY_SIZE * 8 + 1];
            //strncpy(buffer, str, POLYNOMIAL_ARRAY_SIZE* 8);
            str.CopyTo(buffer);

            buffer[POLYNOMIAL_ARRAY_SIZE * 8] = '\0';

            int len = strlen(buffer);

            int pos = len - 8;
            int i;
            for (i = 0; pos >= 0 && i < POLYNOMIAL_ARRAY_SIZE; i++)
            {
                poly[i] = strtoul(buffer, pos);
                buffer[pos] = (char) 0;
                pos -= 8;
            }

            if (pos + 8 > 0)
            {
                poly[i] = strtoul(buffer, 0);
                i++;
            }

            for (; i < POLYNOMIAL_ARRAY_SIZE; i++)
            {
                poly[i] = 0;
            }
        }

        static void polynomialtostr(Span<char> str, Span<uint> poly)
        {
            int first = 1;
            int pos = 0;
            for (int i = POLYNOMIAL_ARRAY_SIZE - 1; i >= 0; i--)
            {
                if (first!=0)
                {
                    if (poly[i] != 0)
                    {
                        sprintf(str, pos, "x", poly[i]);
                        pos = strlen(str);
                        first = 0;
                    }
                }
                else
                {
                    sprintf(str , pos, "x08", poly[i]);
                    pos = strlen(str);
                }
            }
            if (first!=0)
            {
                for(var i=0;i<str.Length;++i)
                {
                    str[i] = '0';
                }
            }
        }

        static void polynomial_power_mod(Span<uint> dest, 
        ReadOnlySpan<uint> x,
            Span<ushort> power,
        ReadOnlySpan<uint> mod_poly)
        {
            Span<uint> tmp = stackalloc uint[POLYNOMIAL_ARRAY_SIZE];
            Span<uint> result = stackalloc uint[POLYNOMIAL_ARRAY_SIZE];

            x.CopyTo(tmp);

            clear(result);
            result[0] = 1;

            for (int i = 0; i<POLYNOMIAL_ARRAY_SIZE; i++)
            {
                ushort tmp_power = power[i];
                for (int j = 0; j< 16; j++) {
                    if ((tmp_power & 1) != 0) {
                        mul(result, tmp);
                        mod(result, mod_poly);
                    }
                    square(tmp);
                    mod(tmp, mod_poly);
                    tmp_power = (ushort)(tmp_power >> 1);
                }
            }

            result.CopyTo(dest);
        }

        static void uz_clear(Span<ushort> a)
        {
            for (int i = 0; i < UZ_ARRAY_SIZE; i++)
            {
                a[i] = 0;
            }
        }

        static void uint32touz(Span<ushort> x, uint y)
        {
            uz_clear(x);
            x[0] = (ushort)(y & 0xffff);
            x[1] = (ushort)(y >> 16);
        }

        static void uz_mul(Span<ushort> result, ReadOnlySpan<ushort> x, ReadOnlySpan<ushort> y)
        {
            uint tmp;
            const uint lmask = 0xffffU;

            uz_clear(result);
            for (int i = 0; i < UZ_ARRAY_SIZE; i++)
            {
                for (int j = 0; j < UZ_ARRAY_SIZE; j++)
                {
                    tmp = (uint) (x[i] * y[j]);
                    if (i + j >= UZ_ARRAY_SIZE)
                    {
                        break;
                    }
                    tmp += result[i + j];
                    result[i + j] = (ushort)(tmp & lmask);
                    for (int k = i + j + 1; k < UZ_ARRAY_SIZE; k++)
                    {
                        tmp = tmp >> 16;
                        if (tmp == 0)
                        {
                            break;
                        }
                        tmp += result[k];
                        result[k] = (ushort) (tmp & lmask);
                    }
                }
            }
        }

        static void string16touz(Span<ushort> result, ReadOnlySpan<char> str)
        {
            Span<char> s = stackalloc char[4 * UZ_ARRAY_SIZE + 1];

            for (var i = 0; i < s.Length; ++i) s[i] = (char) 0;

            //strncpy(s, str, 4 * UZ_ARRAY_SIZE);

            for (int i = 0; i < str.Length; i++)
            {
                s[i] = str[i];
            }
            


            s[4 * UZ_ARRAY_SIZE] = '\0';
            int len = strlen(s);
            uz_clear(result);
            for (int i = 0; i < UZ_ARRAY_SIZE; i++)
            {
                len = len - 4;
                if (len < 0)
                {
                    len = 0;
                }

                ushort tmp = (ushort) strtoul(s, len);
                result[i] = tmp;
                if (len == 0)
                {
                    break;
                }
                s[len] = '\0';
            }
        }



}
}
