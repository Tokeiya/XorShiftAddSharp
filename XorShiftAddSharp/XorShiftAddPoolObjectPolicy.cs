using Microsoft.Extensions.ObjectPool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace XorShiftAddSharp
{
	/// <summary>
	/// Create range 2^96 XorShiftAdd instance.
	/// </summary>
	public sealed class XorShiftAddPoolObjectPolicy : PooledObjectPolicy<XorShiftAdd>
	{
		/// <summary>
		/// Polynomial string of 2^96 jumping.
		/// </summary>
		private const string JumpStr = "5d9ae8e063f5deee4fd1583cf8f7f9d5";

		private InternalState _state;

		public XorShiftAddPoolObjectPolicy(uint initialSeed) => XorShiftAddCore.Init(ref _state, initialSeed);

		public XorShiftAddPoolObjectPolicy(IReadOnlyList<uint> keys) => XorShiftAddCore.Init(ref _state, keys.ToArray());

		public XorShiftAddPoolObjectPolicy(in InternalState initialState) => _state = initialState;

		public override XorShiftAdd Create()
		{
			var ret = XorShiftAdd.Restore(_state);
			XorShiftAddCore.Jump(ref _state, JumpStr);

			return ret;
		}

		public override bool Return(XorShiftAdd obj) => true;
		public InternalState GetCurrentState() => _state;
	}
}
