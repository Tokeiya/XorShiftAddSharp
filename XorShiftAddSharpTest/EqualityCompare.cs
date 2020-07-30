using System;
using System.Collections.Generic;
using XorShiftAddSharp;

namespace XorShiftAddSharpTest
{
	public class InternalStateEqualityCompare : IEqualityComparer<InternalState>
	{
		public static InternalStateEqualityCompare Default { get; }= new InternalStateEqualityCompare();

		public bool Equals(InternalState x, InternalState y)
		{
			var flg = true;

			for (int i = 0; i < InternalState.Size; i++)
			{
				flg &= (x[i] == y[i]);
			}

			return flg;
		}

		public int GetHashCode(InternalState obj)
		{
			var ret = obj[0];

			for (int i = 1; i < InternalState.Size; i++)
			{
				ret ^= obj[i];
			}

			return (int) ret;
		}
	}

	public class XorAddEqualityComparer : IEqualityComparer<XorShiftAdd>
	{
		public static XorAddEqualityComparer Default { get; }=new XorAddEqualityComparer();

		private static readonly InternalStateEqualityCompare Compare = new InternalStateEqualityCompare();

		public bool Equals(XorShiftAdd? x, XorShiftAdd? y)
		{
			if (x is null && y is null) return true;
			if (x is null || y is null) return false;
			return Compare.Equals(x.GetCurrentState(), y.GetCurrentState());

		}

		public int GetHashCode(XorShiftAdd? obj)
		{
			if (obj is null) throw new ArgumentNullException(nameof(obj));

			return Compare.GetHashCode(obj.GetCurrentState());

		}
	}
}