using Microsoft.Extensions.ObjectPool;
using System;

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

		public XorShiftAddPoolObjectPolicy(uint initialSeed)
		{
#warning XorShiftAddPoolObjectPolicy_Is_NotImpl
			throw new NotImplementedException("XorShiftAddPoolObjectPolicy is not implemented");
		}

		public XorShiftAddPoolObjectPolicy(uint[] keys)
		{
#warning XorShiftAddPool_Is_NotImpl
			throw new NotImplementedException("XorShiftAddPool is not implemented");

		}

		public XorShiftAddPoolObjectPolicy(InternalState initialState)
		{
#warning XorShiftAddPoolObjectPolicy_Is_NotImpl
			throw new NotImplementedException("XorShiftAddPoolObjectPolicy is not implemented");
		}

		public override XorShiftAdd Create()
		{
#warning Create_Is_NotImpl
			throw new NotImplementedException("Create is not implemented");
		}

		public override bool Return(XorShiftAdd obj)
		{
#warning Return_Is_NotImpl
			throw new NotImplementedException("Return is not implemented");

		}

		public InternalState GetCurrentState()
		{
#warning GetCurrentState_Is_NotImpl
			throw new NotImplementedException("GetCurrentState is not implemented");
		}
	}
}
