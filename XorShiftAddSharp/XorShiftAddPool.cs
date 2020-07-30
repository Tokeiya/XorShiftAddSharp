using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.Extensions.ObjectPool;

namespace XorShiftAddSharp
{
	/// <summary>
	/// A XorShiftAdd objects pool.
	/// This one provides the 2^96 range splitted instance.(Thus the number of total available instance is 2^32)
	/// </summary>
	public class XorShiftAddPool : ObjectPool<XorShiftAdd>
	{
		private XorShiftAdd? _firstItem;
		private readonly XorShiftAdd?[] _items;
		private readonly IXorShiftAddPoolObjectPolicy _policy;
		private readonly XorShiftAddPoolObjectPolicy? _fastpolicy;
		private readonly bool _isDefaultPolicy;



		public XorShiftAddPool(uint initialSeed) : this(initialSeed, Environment.ProcessorCount * 2)
		{
		}

		public XorShiftAddPool(uint initialSeed, int maximumRetained) : this(
			new XorShiftAddPoolObjectPolicy(initialSeed), maximumRetained)
		{
		}

		public XorShiftAddPool(IReadOnlyList<uint> keys) : this(new XorShiftAddPoolObjectPolicy(keys),
			Environment.ProcessorCount * 2)
		{
		}

		public XorShiftAddPool(IReadOnlyList<uint> keys, int maximumRetained) : this(
			new XorShiftAddPoolObjectPolicy(keys), maximumRetained)
		{
		}

		public XorShiftAddPool(InternalState initialState, IReadOnlyList<InternalState> initialItems) : this(initialState,
			initialItems, Environment.ProcessorCount * 2)
		{
		}

		public XorShiftAddPool(InternalState initialState, IReadOnlyList<InternalState> initialItems,
			int maximumRetained) : this(new XorShiftAddPoolObjectPolicy(initialState), maximumRetained)
		{
			if (maximumRetained <= 0)
				throw new ArgumentException($"{nameof(maximumRetained)} needs to be greater than 0.");
			if (initialItems.Count > maximumRetained)
				throw new ArgumentException($"{nameof(maximumRetained)} < {nameof(initialItems.Count)}");

			_items = new XorShiftAdd[maximumRetained - 1];
			_firstItem = XorShiftAdd.Restore(initialItems[0]);

			for (int i = 1; i < initialItems.Count; i++)
			{
				_items[i - 1] = XorShiftAdd.Restore(initialItems[i]);
			}
		}

		public XorShiftAddPool(IXorShiftAddPoolObjectPolicy policy) : this(policy, Environment.ProcessorCount * 2)
		{
		}

		public XorShiftAddPool(IXorShiftAddPoolObjectPolicy policy, int maximumRetained)
		{
			if (maximumRetained <= 0)
				throw new ArgumentException($"{nameof(maximumRetained)} needs to be greater than 0.");

			_policy = policy;
			_fastpolicy = policy as XorShiftAddPoolObjectPolicy;

			_items = new XorShiftAdd[maximumRetained - 1];
			_isDefaultPolicy = policy is XorShiftAddPoolObjectPolicy;
		}

		public override XorShiftAdd Get()
		{
			var val = _firstItem;

			if (val is null || Interlocked.CompareExchange(ref _firstItem, null, val) != val)
			{
				var items = _items;

				for (int i = 0; i < items.Length; i++)
				{
					val = items[i];
					if (val != null && Interlocked.CompareExchange(ref items[i], null, val) == val)
					{
						return val;
					}
				}

				val = Create();
			}

			return val;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private XorShiftAdd Create() => _fastpolicy?.Create() ?? _policy.Create();

		public override void Return(XorShiftAdd obj)
		{
			if (_isDefaultPolicy || (_fastpolicy?.Return(obj) ?? _policy.Return(obj)))
			{
				if (_firstItem != null || Interlocked.CompareExchange(ref _firstItem, obj, null) != null)
				{
					var items = _items;
					for (int i = 0;
						i < items.Length && Interlocked.CompareExchange(ref items[i], obj, null) != null;
						i++)
					{
					}
				}
			}
		}

		public InternalState GetCurrentState() => _policy.GetCurrentState();


		public IReadOnlyList<InternalState> GetCurrentItems()
		{
			var ret=new List<InternalState>();

			if(_firstItem !=null) ret.Add(_firstItem.GetCurrentState());

			foreach (var elem in _items.Where(x=>x!=null).Select(x=>x!.GetCurrentState()))
			{
				ret.Add(elem);				
			}

			return ret;
		}

	}
}
