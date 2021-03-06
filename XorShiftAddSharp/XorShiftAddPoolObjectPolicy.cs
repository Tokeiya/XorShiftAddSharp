﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.ObjectPool;

namespace XorShiftAddSharp
{
	/// <summary>
	///     Create range 2^64 XorShiftAdd instance.
	/// </summary>
	public sealed class XorShiftAddPoolObjectPolicy : PooledObjectPolicy<XorShiftAdd>, IXorShiftAddPoolObjectPolicy
	{
		/// <summary>
		///     Polynomial string of 2^64 jumping.
		/// </summary>
		private const string JumpStr = "ad97ad554a3f3aa87bacae76fe10e86d";

		private InternalState _state;

		/// <summary>
		///     Constructor.
		/// </summary>
		/// <param name="initialSeed">Specify the initial seed.</param>
		public XorShiftAddPoolObjectPolicy(uint initialSeed)
		{
			XorShiftAddCore.Init(out _state, initialSeed);
		}


		/// <summary>
		///     Constructor.
		/// </summary>
		/// <param name="keys">Specify the initial keys.</param>
		public XorShiftAddPoolObjectPolicy(IReadOnlyList<uint> keys)
		{
			XorShiftAddCore.Init(out _state, keys.ToArray());
		}

		/// <summary>
		///     Constructor.
		/// </summary>
		/// <param name="initialState">Specify the initial state.</param>
		public XorShiftAddPoolObjectPolicy(in InternalState initialState)
		{
			_state = initialState;
		}

		/// <summary>
		///     Create range 2^96 XorShiftAdd instance.
		/// </summary>
		/// <returns>
		///     Return the created instance.
		/// </returns>
		public override XorShiftAdd Create()
		{
			var ret = XorShiftAdd.Restore(_state);
			XorShiftAddCore.Jump(ref _state, JumpStr);

			return ret;
		}


		/// <summary>
		///     Return the instance.
		/// </summary>
		/// <param name="obj">Specify the instance want to return.</param>
		/// <returns>
		///     Always return true.
		/// </returns>
		public override bool Return(XorShiftAdd obj)
		{
			return true;
		}

		public InternalState GetCurrentState()
		{
			return _state;
		}
	}
}