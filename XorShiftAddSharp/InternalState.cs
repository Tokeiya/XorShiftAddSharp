using System;
using System.Collections.Generic;

namespace XorShiftAddSharp
{
    /// <summary>
    /// XorShiftAdd internal state storage.
    /// </summary>
    public unsafe struct InternalState
    {
        /// <summary>
        /// Internal state size.
        /// </summary>
        public const int Size = 4;

        /// <summary>
        /// State
        /// </summary>
        public fixed uint State[Size];

        /// <summary>
        /// Generate XorShiftAdd, initialized by specified source.
        /// </summary>
        /// <param name="source">
        /// Specify thr source for initialization.
        /// </param>
        /// <returns>
        /// XorShiftAddState that initialized by specified source.
        /// </returns>
        public static InternalState Initialize(ReadOnlySpan<uint> source)
        {
            if (source.Length != Size)
                throw new ArgumentException($"Unexpected {nameof(source)} Length");

            var ret = new InternalState();

            for (int i = 0; i < Size; i++) ret.State[i] = source[i];

            return ret;
        }

        /// <summary>
        /// Generate XorShiftAdd, initialized by specified source.
        /// </summary>
        /// <param name="source">
        /// Specify thr source for initialization.
        /// </param>
        /// <returns>
        /// XorShiftAddState that initialized by specified source.
        /// </returns>
        public static InternalState Initialize(IReadOnlyList<uint> source)
        {
            if (source.Count != Size)
                throw new ArgumentException($"Unexpected {nameof(source)} Count");

            var ret = new InternalState();

            for (int i = 0; i < Size; i++) ret.State[i] = source[i];

            return ret;
        }

        /// <summary>
        /// Get or set the internal state at the specified index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of element to get or set.
        /// </param>
        public uint this[int index]
        {
            get
            {
                if ((uint)index >= Size) throw new IndexOutOfRangeException();
                return State[index];
            }
            set
            {
                if ((uint)index >= Size) throw new IndexOutOfRangeException();
                State[index] = value;
            }
        }

        /// <summary>
        /// Copy internal internal state to the specified destination.
        /// </summary>
        /// <param name="destination">
        /// Specify the destination XorShiftAddState that are copied.
        /// </param>
        public void CopyTo(ref InternalState destination)
        {
            for (int i = 0; i < Size; i++) destination.State[i] = State[i];
        }


        /// <summary>
        /// Crates an copied array.
        /// </summary>
        /// <returns>
        /// An array that copied.
        /// </returns>
        public uint[] ToArray()
        {
            var ret = new uint[Size];

            for (int i = 0; i < Size; i++) ret[i] = State[i];

            return ret;
        }
    }
}