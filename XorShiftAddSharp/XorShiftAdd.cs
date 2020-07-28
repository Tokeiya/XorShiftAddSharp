using System;
using System.Collections.Generic;

namespace XorShiftAddSharp
{
    /// <summary>
    /// XORSHIFT-ADD wrapper.
    /// </summary>
    public sealed class XorShiftAdd : Random
    {
        private InternalState _state;

        private XorShiftAdd(in InternalState initialState)
        {
            _state = initialState;
        }

        /// <summary>
        /// Initializes the internal state array with a 32-bit unsigned integer seed.
        /// </summary>
        /// <param name="seed">A 32-bit unsigned integer used as a seed.</param>
        public XorShiftAdd(uint seed)
        {
            XorShiftAddCore.Init(ref _state, seed);
        }

        /// <summary>
        ///     Initializes the internal state array, with an array of 32-bit unsigned integers used as seeds.
        /// </summary>
        /// <param name="seeds">The array of 32-bit integers, used as a seed.</param>
        public XorShiftAdd(ReadOnlySpan<uint> seeds)
        {
            XorShiftAddCore.Init(ref _state, seeds);
        }

        public InternalState GetCurrentState() => _state;


        /// <summary>
        /// calculate jump polynomial.
        /// </summary>
        /// <param name="mulStep">jump step is mul_step * base_step.</param>
        /// <param name="baseStep">hexadecimal string of jump base.</param>
        /// <returns>the result of this calculation.</returns>
        public static string CalculateJumpPolynomial(uint mulStep, string baseStep)
        {
            try
            {
                Span<char> buff = stackalloc char[XorShiftAddCore.JumpStrSize];
                XorShiftAddCore.CalculateJumpPolynomial(buff, mulStep, baseStep);

                int len;

                for (len = 0; len < buff.Length; len++)
                    if (buff[len] == '\0')
                        break;

                return new string(buff[..len]);
            }
            catch (FormatException ex)
            {
                throw new ArgumentException($"{baseStep} is unexpected value.", ex);
            }
        }


        /// <summary>
        /// Restore the pseudo-random generator from specified state.
        /// </summary>
        /// <param name="state">Specify the state to restore.</param>
        /// <returns>pseudo-random generator that was restored from specified internal state.</returns>
        public static XorShiftAdd Restore(InternalState state) => new XorShiftAdd(state);

        /// <summary>
        /// Restore the pseudo-random generator from specified state.
        /// </summary>
        /// <param name="state">Specify the state to restore.</param>
        /// <returns>pseudo-random generator that was restored from specified internal state.</returns>
        public static XorShiftAdd Restore(IReadOnlyList<uint> state)
        {
            if (state.Count != InternalState.Size)
                throw new ArgumentException($"{nameof(state)} size is unexpected.");

            return new XorShiftAdd(InternalState.Initialize(state));
        }


        /// <summary>
        /// Output 32-bit unsigned  integer pseudorandom number.
        /// </summary>
        /// <returns>[0..uint.MaxValue]</returns>
        public uint NextUnsignedInt()
        {
            return XorShiftAddCore.NextUint32(ref _state);
        }


        /// <summary>
        /// Output the 23bit resolution float pseudorandom number.
        /// </summary>
        /// <returns>[0.0..1.0)</returns>
        public float NextFloat()
        {
            return XorShiftAddCore.NextFloat(ref _state);
        }

        /// <summary>
        /// Output 32-bit singed integer positive pseudorandom  number
        /// </summary>
        /// <returns>[0..)</returns>
        public override int Next()
        {
            const uint mask = int.MaxValue;


            for (;;)
            {
                var tmp = XorShiftAddCore.NextUint32(ref _state);
                tmp &= mask;
                if (tmp == int.MaxValue) continue;

                return (int) tmp;
            }
        }


        /// <summary>
        /// Output [minvalue..maxValue) 32-bit signed integer pseudorandom number.
        /// </summary>
        /// <param name="minValue">Specify the minimum value that inclusive.</param>
        /// <param name="maxValue">Specify the maxim value that exclusive.</param>
        /// <returns>[minValue..maxValue)</returns>
        public override int Next(int minValue, int maxValue)
        {
            if (maxValue < minValue)
                throw new ArgumentOutOfRangeException(nameof(minValue), "minValue and maxValue was inverted.");

            long diff = (long) maxValue - minValue;
            return (int) ((long) (Sample() * diff) + minValue);
        }

        /// <summary>
        /// Retrieves a new XorShiftAdd jumped from this instance.
        /// </summary>
        /// <param name="mulStep">Specify the jump step. that is used by mul_step * base_step.</param>
        /// <param name="baseStep">Specify the hexadecimal number string less than 2^128.</param>
        /// <returns>New XorShiftAdd that internal state was jumped. </returns>
        public XorShiftAdd Jump(uint mulStep, string baseStep)
        {
            InternalState tmp = new InternalState();
            _state.CopyTo(ref tmp);

            XorShiftAddCore.Jump(ref tmp, mulStep, baseStep);

            //return Restore(ref tmp);
            return new XorShiftAdd(tmp);
        }

        /// <summary>
        /// Jump the internal state.
        /// </summary>
        /// <param name="jumpStr">the jump polynomial. Calculated by CalculateJumpPolynomial method.</param>
        /// <returns>New XorShiftAdd that internal state was jumped.</returns>
        public XorShiftAdd Jump(string jumpStr)
        {
            InternalState tmp = new InternalState();
            _state.CopyTo(ref tmp);

            XorShiftAddCore.Jump(ref tmp, jumpStr);

            return new XorShiftAdd(tmp);
        }

        /// <summary>
        /// Output [0..maxValue) 32-bit singed integer pseudorandom number.
        /// </summary>
        /// <param name="maxValue">Specify the maxim value that exclusive.</param>
        /// <returns>[0..maxValue)</returns>
        public override int Next(int maxValue)
        {
            if (maxValue < 0) throw new ArgumentOutOfRangeException(nameof(maxValue), "maxValue is negative");

            return (int) (Sample() * maxValue);
        }

        /// <summary>
        /// Fill the byte array with pseudorandom bytes [0..byte.MaxValue).
        /// </summary>
        /// <param name="buffer">Specify the array to be filled.</param>
        public override void NextBytes(byte[] buffer)
        {
            for (int i = 0; i < buffer.Length; i++) buffer[i] = (byte) XorShiftAddCore.NextUint32(ref _state);
        }

        /// <summary>
        /// Fill the Span&lt;byte&gt; with pseudorandom bytes [0..byte.MaxValue).
        /// </summary>
        /// <param name="buffer">Specify the Span&lt;byte&gt; to be filled.</param>
        public override void NextBytes(Span<byte> buffer)
        {
            for (int i = 0; i < buffer.Length; i++) buffer[i] = (byte) XorShiftAddCore.NextUint32(ref _state);
        }


        /// <summary>
        /// Output [0.0..1.0) 53bit resolution double value.
        /// </summary>
        /// <returns>[0..1)</returns>
        public override double NextDouble()
        {
            return XorShiftAddCore.NextDouble(ref _state);
        }

        /// <summary>
        ///     Output [0..1) double value.
        /// </summary>
        /// <returns>[0..1)</returns>
        protected override double Sample()
        {
            return XorShiftAddCore.NextDouble(ref _state);
        }
    }
}