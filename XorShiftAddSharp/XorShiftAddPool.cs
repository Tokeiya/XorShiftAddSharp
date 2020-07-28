using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.ObjectPool;

namespace XorShiftAddSharp
{
	/// <summary>
	/// A XorShiftAdd objects pool.
	/// This one provides the 2^96 range splitted instance.(Thus the number of total available instance is 2^32)
	/// </summary>
	public class XorShiftAddPool : ObjectPool<XorShiftAdd>
	{
		public XorShiftAddPool(uint initialSeed)
		{
#warning XorShiftAddPool_Is_NotImpl
			throw new NotImplementedException("XorShiftAddPool is not implemented");
		}

		public XorShiftAddPool(uint initialSeed, int maximumRetained)
		{
#warning XorShiftAddPool_Is_NotImpl
			throw new NotImplementedException("XorShiftAddPool is not implemented");
		}

		public XorShiftAddPool(IReadOnlyList<uint> keys)
		{
#warning XorShiftAddPool_Is_NotImpl
			throw new NotImplementedException("XorShiftAddPool is not implemented");
		}

		public XorShiftAddPool(IReadOnlyList<uint> keys, int maximumRetained)
		{
#warning XorShiftAddPool_Is_NotImpl
			throw new NotImplementedException("XorShiftAddPool is not implemented");
		}

		public XorShiftAddPool(InternalState initialState, IEnumerable<InternalState> initialItems)
		{
#warning XorShiftAddPool_Is_NotImpl
			throw new NotImplementedException("XorShiftAddPool is not implemented");
		}

		public XorShiftAddPool(InternalState initialState, IEnumerable<InternalState> initialItems, int maximumRetained)
		{
#warning XorShiftAddPool_Is_NotImpl
			throw new NotImplementedException("XorShiftAddPool is not implemented");
		}

		public XorShiftAddPool(IPooledObjectPolicy<XorShiftAdd> policy)
		{
#warning XorShiftAddPool_Is_NotImpl
			throw new NotImplementedException("XorShiftAddPool is not implemented");
		}

		public XorShiftAddPool(IPooledObjectPolicy<XorShiftAdd> policy, int maximumRetained)
		{
#warning XorShiftAddPool_Is_NotImpl
			throw new NotImplementedException("XorShiftAddPool is not implemented");
		}

		public override XorShiftAdd Get()
		{
#warning Get_Is_NotImpl
			throw new NotImplementedException("Get is not implemented");
		}

		public override void Return(XorShiftAdd obj)
		{
#warning Return_Is_NotImpl
			throw new NotImplementedException("Return is not implemented");
		}

		public InternalState GetCurrentState()
		{
#warning GetCurrentState_Is_NotImpl
			throw new NotImplementedException("GetCurrentState is not implemented");

		}

		public IEnumerable<InternalState> GetCurrentItems()
		{
#warning GetCurrentItems_Is_NotImpl
			throw new NotImplementedException("GetCurrentItems is not implemented");
		}
	}
}
