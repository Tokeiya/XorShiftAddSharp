using System;
using System.Collections.Generic;

namespace XorShiftAddSharp
{
    /// <summary>
    /// XorShiftAdd internal vector storage.
    /// </summary>
    public unsafe struct XorShiftAddState
    {
        /// <summary>
        /// Vector size.
        /// </summary>
        public const int Size = 4;

        /// <summary>
        /// Storage.
        /// </summary>
        public fixed uint Vector[Size];

        /// <summary>
        /// Generate XorShiftAdd, initialized by specified source.
        /// </summary>
        /// <param name="source">
        /// Specify thr source for initialization.
        /// </param>
        /// <returns>
        /// XorShiftAddState that initialized by specified source.
        /// </returns>
        internal static XorShiftAddState Initialize(ReadOnlySpan<uint> source)
        {
            if (source.Length != Size)
                throw new ArgumentException($"Unexpected {nameof(source)} Length");

            var ret = new XorShiftAddState();

            for (int i = 0; i < Size; i++) ret.Vector[i] = source[i];

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
        internal static XorShiftAddState Initialize(IReadOnlyList<uint> source)
        {
            if (source.Count != Size)
                throw new ArgumentException($"Unexpected {nameof(source)} Count");

            var ret = new XorShiftAddState();

            for (int i = 0; i < Size; i++) ret.Vector[i] = source[i];

            return ret;
        }

        /// <summary>
        /// Get or set the vector state at the specified index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of element to get or set.
        /// </param>
        public uint this[int index]
        {
            get
            {
                if ((uint)index >= Size) throw new IndexOutOfRangeException();
                return Vector[index];
            }
            set
            {
                if ((uint)index >= Size) throw new IndexOutOfRangeException();
                Vector[index] = value;
            }
        }

        /// <summary>
        /// Copy internal vector state to the specified destination.
        /// </summary>
        /// <param name="destination">
        /// Specify the destination XorShiftAddState that are copied.
        /// </param>
        public void CopyTo(ref XorShiftAddState destination)
        {
            for (int i = 0; i < Size; i++) destination.Vector[i] = Vector[i];
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

            for (int i = 0; i < Size; i++) ret[i] = Vector[i];

            return ret;
        }
    }
}