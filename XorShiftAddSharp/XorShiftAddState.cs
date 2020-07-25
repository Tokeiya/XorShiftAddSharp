using System;
using System.Collections.Generic;
using System.Text;

namespace XorShiftAddSharp
{
    public unsafe struct XorShiftAddState
    {
        internal static XorShiftAddState Initialize(ReadOnlySpan<uint> source)
        {
            if (source.Length != XorShiftAddCore.InnerVectorSize)
                throw new ArgumentException($"Unexpected {nameof(source)} Length");
            
            var ret=new XorShiftAddState();

            for (int i = 0; i < XorShiftAddCore.InnerVectorSize; i++)
            {
                ret.Vector[i] = source[i];
            }

            return ret;
        }

        internal static XorShiftAddState Initialize(IReadOnlyList<uint> source)
        {
            if (source.Count != XorShiftAddCore.InnerVectorSize)
                throw new ArgumentException($"Unexpected {nameof(source)} Count");

            var ret = new XorShiftAddState();

            for (int i = 0; i < XorShiftAddCore.InnerVectorSize; i++)
            {
                ret.Vector[i] = source[i];
            }

            return ret;

        }

        public fixed uint Vector[XorShiftAddCore.InnerVectorSize];

        public void CopyTo(ref XorShiftAddState destination)
        {
            for (int i = 0; i < XorShiftAddCore.InnerVectorSize; i++)
            {
                destination.Vector[i] = Vector[i];
            }
        }

        public IReadOnlyList<uint> ToReadOnlyList()
        {
            var ret = new uint[XorShiftAddCore.InnerVectorSize];

            for (int i = 0; i < XorShiftAddCore.InnerVectorSize; i++)
            {
                ret[i] = Vector[i];
            }

            return ret;
        }
    }
}
