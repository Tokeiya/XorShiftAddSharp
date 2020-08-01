using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.Extensions.ObjectPool;

namespace XorShiftAddSharp
{
	/// <summary>
	///     A XorShiftAdd objects pool.
	///     This one provides the 2^64 range splitted instance.(Thus the number of total available instance is 2^64)
	/// </summary>
	public class XorShiftAddPool : ObjectPool<XorShiftAdd>
	{
		private readonly XorShiftAddPoolObjectPolicy? _fastpolicy;
		private readonly bool _isDefaultPolicy;
		private readonly XorShiftAdd?[] _items;
		private readonly IXorShiftAddPoolObjectPolicy _policy;
		private XorShiftAdd? _firstItem;


		/// <summary>
		///     Ctor.
		/// </summary>
		/// <param name="seed">Specify the initial seed.</param>
		public XorShiftAddPool(uint seed) : this(seed, Environment.ProcessorCount * 2)
		{
		}

		/// <summary>
		///     Ctor.
		/// </summary>
		/// <param name="seed">Specify the initial seed.</param>
		/// <param name="maximumRetained">The maximum number of objects to retain in the pool.</param>
		public XorShiftAddPool(uint seed, int maximumRetained) : this(
			new XorShiftAddPoolObjectPolicy(seed), maximumRetained)
		{
		}

		/// <summary>
		///     Ctor.
		/// </summary>
		/// <param name="keys">Specify the initial keys.</param>
		public XorShiftAddPool(IReadOnlyList<uint> keys) : this(new XorShiftAddPoolObjectPolicy(keys),
			Environment.ProcessorCount * 2)
		{
		}

		/// <summary>
		///     Ctor
		/// </summary>
		/// <param name="keys">Specify the initial keys.</param>
		/// <param name="maximumRetained">The maximum number of objects to retain in the pool.</param>
		public XorShiftAddPool(IReadOnlyList<uint> keys, int maximumRetained) : this(
			new XorShiftAddPoolObjectPolicy(keys), maximumRetained)
		{
		}

		/// <summary>
		///     Ctor
		/// </summary>
		/// <param name="initialState">Specify the initial state.</param>
		/// <param name="initialItems">Specify the initial items that stored.</param>
		public XorShiftAddPool(InternalState initialState, IReadOnlyList<InternalState> initialItems) : this(
			initialState,
			initialItems, Environment.ProcessorCount * 2)
		{
		}


		/// <summary>
		///     Ctor.
		/// </summary>
		/// <param name="initialState">Specify the initial state.</param>
		/// <param name="initialItems">Specify the initial items that stored.</param>
		/// <param name="maximumRetained">The maximum number of objects to retain in the pool.</param>
		public XorShiftAddPool(InternalState initialState, IReadOnlyList<InternalState> initialItems,
			int maximumRetained) : this(new XorShiftAddPoolObjectPolicy(initialState), maximumRetained)
		{
			if (maximumRetained <= 0)
				throw new ArgumentException($"{nameof(maximumRetained)} needs to be greater than 0.");
			if (initialItems.Count > maximumRetained)
				throw new ArgumentException($"{nameof(maximumRetained)} < {nameof(initialItems.Count)}");

			_items = new XorShiftAdd[maximumRetained - 1];
			_firstItem = XorShiftAdd.Restore(initialItems[0]);

			for (int i = 1; i < initialItems.Count; i++) _items[i - 1] = XorShiftAdd.Restore(initialItems[i]);
		}


		/// <summary>
		///     Ctor.
		/// </summary>
		/// <param name="policy">Specify the policy that using.</param>
		public XorShiftAddPool(IXorShiftAddPoolObjectPolicy policy) : this(policy, Environment.ProcessorCount * 2)
		{
		}

		/// <summary>
		///     Ctor.
		/// </summary>
		/// <param name="policy">Specify the policy that using.</param>
		/// <param name="maximumRetained">The maximum number of objects to retain in the pool.</param>
		public XorShiftAddPool(IXorShiftAddPoolObjectPolicy policy, int maximumRetained)
		{
			if (maximumRetained <= 0)
				throw new ArgumentException($"{nameof(maximumRetained)} needs to be greater than 0.");

			_policy = policy;
			_fastpolicy = policy as XorShiftAddPoolObjectPolicy;

			_items = new XorShiftAdd[maximumRetained - 1];
			_isDefaultPolicy = policy is XorShiftAddPoolObjectPolicy;
		}


		/// <summary>
		///     Get the XorShiftAdd instance that jumped to 2^64 steps from previous created instance.
		/// </summary>
		/// <returns>XorShiftAdd instance that jumped to 2^64 steps from previous created instance.</returns>
		public override XorShiftAdd Get()
		{
			var val = _firstItem;

			if (val is null || Interlocked.CompareExchange(ref _firstItem, null, val) != val)
			{
				var items = _items;

				for (int i = 0; i < items.Length; i++)
				{
					val = items[i];
					if (val != null && Interlocked.CompareExchange(ref items[i], null, val) == val) return val;
				}

				val = Create();
			}

			return val;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private XorShiftAdd Create()
		{
			return _fastpolicy?.Create() ?? _policy.Create();
		}


		/// <summary>
		///     Return the instance.
		/// </summary>
		/// <param name="obj">Specify the instance that return.</param>
		public override void Return(XorShiftAdd obj)
		{
			if (_isDefaultPolicy || (_fastpolicy?.Return(obj) ?? _policy.Return(obj)))
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


		/// <summary>
		///     Get the current InternalState.
		/// </summary>
		/// <returns>Current InternalState.</returns>
		public InternalState GetCurrentState()
		{
			return _policy.GetCurrentState();
		}


		/// <summary>
		///     Get the current stored XorShiftAdd instance's InternalState.
		/// </summary>
		/// <returns>Current stored XorShiftAdd instance's InternalState.</returns>
		public IReadOnlyList<InternalState> GetCurrentItems()
		{
			var ret = new List<InternalState>();

			if (_firstItem != null) ret.Add(_firstItem.GetCurrentState());

			foreach (var elem in _items.Where(x => x != null).Select(x => x!.GetCurrentState())) ret.Add(elem);

			return ret;
		}
	}
}