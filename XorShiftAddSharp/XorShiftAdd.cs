using System;
using System.Collections.Generic;

namespace XorShiftAddSharp
{
    public sealed class XorShiftAdd : Random
    {
        private readonly uint[] _state = new uint[XorShiftAddCore.InnerVectorSize];


        public static string CalculateJumpPolynomial(uint mulStep, string baseStep)
        {
            try
            {
                Span<char> buff = stackalloc char[XorShiftAddCore.JumpStrSize];
                XorShiftAddCore.CalculateJumpPolynomial(buff, mulStep, baseStep);

                int len;

                for (len = 0; len < buff.Length; len++)
                {
                    if (buff[len] == '\0') break;
                }

                return new string(buff[..len]);
            }
            catch (FormatException ex)
            {
                throw new ArgumentException($"{baseStep} is unexpected value.", ex);
            }
        }

        public XorShiftAdd(uint seed) => XorShiftAddCore.Init(_state, seed);
        public XorShiftAdd(ReadOnlySpan<uint> seeds) => XorShiftAddCore.Init(_state, seeds);

        public uint NextUint() => XorShiftAddCore.NextUint32(_state);
        public float NextFloat() => XorShiftAddCore.NextFloat(_state);

        public override int Next()
        {
            const uint mask = int.MaxValue;


            for (;;)
            {
                var tmp = XorShiftAddCore.NextUint32(_state);
                tmp &= mask;
                if (tmp == int.MaxValue) continue;

                return (int) tmp;
            }
        }

        public override int Next(int minValue, int maxValue)
        {
            if (maxValue < minValue) throw new ArgumentOutOfRangeException(nameof(minValue),"minValue and maxValue was inverted.");

            long diff = (long) maxValue - minValue;
            return (int) ((long) (Sample() * diff) + minValue);
        }

        public override int Next(int maxValue)
        {
            if (maxValue < 0) throw new ArgumentOutOfRangeException(nameof(maxValue), "maxValue is negative");

            return (int) (Sample() * maxValue);
        }

        public override void NextBytes(byte[] buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (byte) XorShiftAddCore.NextUint32(_state);
            }
        }

        public override void NextBytes(Span<byte> buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (byte)XorShiftAddCore.NextUint32(_state);
            }
        }

        public override double NextDouble() => XorShiftAddCore.NextDouble(_state);

        protected override double Sample() => XorShiftAddCore.NextDouble(_state);
        public IReadOnlyList<uint> State => _state;

    }
}
