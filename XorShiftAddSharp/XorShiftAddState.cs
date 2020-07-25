using System;
using System.Collections.Generic;

namespace XorShiftAddSharp
{
    public unsafe struct XorShiftAddState
    {
        public const int Size = 4;

        public fixed uint Vector[Size];

        internal static XorShiftAddState Initialize(ReadOnlySpan<uint> source)
        {
            if (source.Length != Size)
                throw new ArgumentException($"Unexpected {nameof(source)} Length");

            var ret = new XorShiftAddState();

            for (int i = 0; i < Size; i++) ret.Vector[i] = source[i];

            return ret;
        }

        internal static XorShiftAddState Initialize(IReadOnlyList<uint> source)
        {
            if (source.Count != Size)
                throw new ArgumentException($"Unexpected {nameof(source)} Count");

            var ret = new XorShiftAddState();

            for (int i = 0; i < Size; i++) ret.Vector[i] = source[i];

            return ret;
        }


        public void CopyTo(ref XorShiftAddState destination)
        {
            for (int i = 0; i < Size; i++) destination.Vector[i] = Vector[i];
        }

        public IReadOnlyList<uint> ToReadOnlyList()
        {
            var ret = new uint[Size];

            for (int i = 0; i < Size; i++) ret[i] = Vector[i];

            return ret;
        }

    }
}