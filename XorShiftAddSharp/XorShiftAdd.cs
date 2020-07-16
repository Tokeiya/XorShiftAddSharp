using System;
using System.Collections.Generic;
using System.Text;

namespace XorShiftAddSharp
{
    public sealed class XorShiftAdd:Random
    {
        private readonly uint[] _state = new uint[XsAddCore.InnerVectorSize];

        private void CopyState(XorShiftAdd copyFrom)
        {
            for (int i = 0; i < _state.Length; i++)
            {
                _state[i] = copyFrom._state[i];
            }
        }

        public static string CalculateJumpPolynomial(uint mulStep, string baseStep)
        {
            try
            {
                Span<char> buff = stackalloc char[XsAddCore.JumpStrSize];
                XsAddCore.CalculateJumpPolynomial(buff, mulStep, baseStep);

                return new string(buff);
            }
            catch (FormatException ex)
            {
                throw new ArgumentException($"{baseStep} is unexpected value.", ex);
            }
        }

        public XorShiftAdd(uint seed)=> XsAddCore.Init(_state, seed);


        public XorShiftAdd(ReadOnlySpan<uint> seeds) => XsAddCore.Init(_state, seeds);

        public XorShiftAdd(XorShiftAdd pivot, string jumpStr)
        {
            CopyState(pivot);

#warning XorShiftAdd_Is_NotImpl
            throw new NotImplementedException("XorShiftAdd is not implemented");
        }


        public XorShiftAdd(XorShiftAdd pivot, uint mulStep, string baseStep)
        {
#warning XorShiftAdd_Is_NotImpl
            throw new NotImplementedException("XorShiftAdd is not implemented");
        }

        public XorShiftAdd Jump(uint mulStep, string baseStep)
        {
#warning Jump_Is_NotImpl
            throw new NotImplementedException("Jump is not implemented");
        }

        public XorShiftAdd Jump(string jumpStr)
        {
#warning Jump_Is_NotImpl
            throw new NotImplementedException("Jump is not implemented");
        }

        public override int Next()
        {
#warning Next_Is_NotImpl
            throw new NotImplementedException("Next is not implemented");
        }

        public override int Next(int minValue, int maxValue)
        {
#warning Next_Is_NotImpl
            throw new NotImplementedException("Next is not implemented");
        }

        public override int Next(int maxValue)
        {
#warning Next_Is_NotImpl
            throw new NotImplementedException("Next is not implemented");
        }

        public override void NextBytes(byte[] buffer)
        {
#warning NextBytes_Is_NotImpl
            throw new NotImplementedException("NextBytes is not implemented");
        }

        public override void NextBytes(Span<byte> buffer)
        {
#warning NextBytes_Is_NotImpl
            throw new NotImplementedException("NextBytes is not implemented");
        }

        public override double NextDouble()
        {
#warning NextDouble_Is_NotImpl
            throw new NotImplementedException("NextDouble is not implemented");
        }

        protected override double Sample()
        {
#warning Sample_Is_NotImpl
            throw new NotImplementedException("Sample is not implemented");
        }
    }
}
