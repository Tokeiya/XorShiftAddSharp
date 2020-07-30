using ChainingAssertion;
using System;
using System.Collections.Generic;
using System.Linq;
using XorShiftAddSharp;
using Xunit;
using Xunit.Abstractions;

namespace XorShiftAddSharpTest
{
	public class DefaultXorShiftAddPoolTest
	{

		private readonly ITestOutputHelper _output;
		public DefaultXorShiftAddPoolTest(ITestOutputHelper output) => _output = output;

		private static readonly int DefaultMaximumRetained = Environment.ProcessorCount * 2;

		static void AreEqual(InternalState actual, InternalState expected)
		{
			for (int i = 0; i < InternalState.Size; i++)
			{
				actual[i].Is(expected[i]);
			}
		}

		static bool Equal(InternalState x, InternalState y)
		{
			var ret = true;
			for (int i = 0; i < InternalState.Size; i++)
			{
				ret &= (x[i] == y[i]);
			}

			return ret;
		}

		static void Verify(XorShiftAddPool actual,in InternalState expectedInternalState,int maximumRetained)
		{
			InternalStateEqualityCompare.Default.Equals(actual.GetCurrentState(),expectedInternalState).IsTrue();

			var first = new HashSet<XorShiftAdd>();

			for (int i = 0; i < maximumRetained + 1; i++)
			{
				var tmp = actual.Get();
				first.Add(tmp);
			}

			foreach (var elem in first)
			{
				actual.Return(elem);
			}

			var second = new List<XorShiftAdd>();

			for (int i = 0; i < maximumRetained; i++)
			{
				var tmp = actual.Get();
				first.Any(x => Equal(x.GetCurrentState(), tmp.GetCurrentState())).IsTrue();
				second.Add(tmp);
			}

			foreach (var elem in second)
			{
				actual.Return(elem);
			}

			var current = actual.GetCurrentItems();
			current.Count.Is(maximumRetained);

			foreach (var elem in current)
			{
				first.Any(x => Equal(x.GetCurrentState(), elem));
			}

		}

		static void Verify(XorShiftAddPool actual, in InternalState expectedInternalState
			,int maximumRetained, IReadOnlyList<InternalState> expected)
		{
			
			var first=actual.GetCurrentItems()
				.Zip(expected, (act, exp) => (Equal(act, exp))).ToArray();

				first.All(x => x).IsTrue();

				Verify(actual,expectedInternalState, maximumRetained);
		}

		[Fact]
		public void SimpleInitTest()
		{
			InternalState expected=new InternalState();

			XorShiftAddCore.Init(out expected, 42);
			var actual = new XorShiftAddPool(42);
			Verify(actual,expected,Environment.ProcessorCount * 2);


			actual = new XorShiftAddPool(42, 32);
			Verify(actual,expected, 32);
			
			Assert.Throws<ArgumentException>(() => new XorShiftAddPool(42, 0));
			Assert.Throws<ArgumentException>(() => new XorShiftAddPool(42, -1));
			Assert.Throws<ArgumentException>(() => new XorShiftAddPool(new XorShiftAddPoolObjectPolicy(42), 0));
			Assert.Throws<ArgumentException>(() => new XorShiftAddPool(new XorShiftAddPoolObjectPolicy(42), -1));

			actual = new XorShiftAddPool(42, 1);
			Verify(actual,expected, 1);

			XorShiftAddCore.Init(out expected,new uint[]{42,810,114514});
			actual = new XorShiftAddPool(new uint[] { 42, 810, 114514 });
			Verify(actual,expected, DefaultMaximumRetained);


			actual = new XorShiftAddPool(new uint[] { 42,810, 114514 }, 32);
			Verify(actual,expected, 32);

			XorShiftAddCore.Init(out expected,42);
			actual = new XorShiftAddPool(new XorShiftAddPoolObjectPolicy(42));
			Verify(actual,expected, DefaultMaximumRetained);

			actual = new XorShiftAddPool(new XorShiftAddPoolObjectPolicy(42), 32);
			Verify(actual,expected, 32);

		}

		[Fact]
		public void RestoreInitTest()
		{
			var initialState = new InternalState {[0] = 1, [1] = 2, [2] = 3, [3] = 4};
			var internalStates = Enumerable.Range(1, DefaultMaximumRetained)
				.Select(i => new InternalState {[0] = (uint) i}).ToArray();

			var actual = new XorShiftAddPool(initialState, internalStates);
			Verify(actual,initialState,DefaultMaximumRetained,internalStates);

			Assert.Throws<ArgumentException>(() =>
				new XorShiftAddPool(initialState, internalStates, DefaultMaximumRetained - 1));

			Assert.Throws<ArgumentException>(() =>
				new XorShiftAddPool(initialState, internalStates, 0));

			Assert.Throws<ArgumentException>(() =>
				new XorShiftAddPool(initialState, internalStates,  - 1));


		}

		[Fact]
		public void GetTest()
		{
			InternalState expected;
			XorShiftAddCore.Init(out expected, 42);

			var actual=new XorShiftAddPool(42);



		}



	}
}
