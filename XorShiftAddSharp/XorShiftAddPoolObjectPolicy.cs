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

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="initialSeed">Specify the initial seed.</param>
		public XorShiftAddPoolObjectPolicy(uint initialSeed) => XorShiftAddCore.Init(ref _state, initialSeed);


		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="keys">Specify the initial keys.</param>
		public XorShiftAddPoolObjectPolicy(IReadOnlyList<uint> keys) => XorShiftAddCore.Init(ref _state, keys.ToArray());

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="initialState">Specify the initial state.</param>
		public XorShiftAddPoolObjectPolicy(in InternalState initialState) => _state = initialState;

		/// <summary>
		/// Create range 2^96 XorShiftAdd instance. 
		/// </summary>
		/// <returns>
		/// Return the created instance.
		/// </returns>
		public override XorShiftAdd Create()
		{
			var ret = XorShiftAdd.Restore(_state);
			XorShiftAddCore.Jump(ref _state, JumpStr);

			return ret;
		}


		/// <summary>
		/// Return the instance.
		/// </summary>
		/// <param name="obj">Specify the instance want to return.</param>
		/// <returns>
		/// Always return true.
		/// </returns>
		public override bool Return(XorShiftAdd obj) => true;
		public InternalState GetCurrentState() => _state;
	}
}
