using System;
using System.Runtime.CompilerServices;
using ChainingAssertion;
using XorShiftAddSharp;
using Xunit;
using Xunit.Abstractions;

namespace XorShiftAddSharpTest
{
	public class XorShiftAddPoolObjectPolicyTest
	{
		private readonly ITestOutputHelper _output;
		public XorShiftAddPoolObjectPolicyTest(ITestOutputHelper output) => _output = output;

		static void Assert(XorShiftAddPoolObjectPolicy actual, in InternalState expected)
		{
			var state = actual.GetCurrentState();

			for (int i = 0; i < InternalState.Size; i++)
			{
				state[i].Is(expected[i]);
			}
		}

		static void Assert(XorShiftAdd actual, in InternalState expected)
		{
			
		}

	[Fact]
		public void CtorTest()
		{
			var actual=new XorShiftAddPoolObjectPolicy(42);

			InternalState expected=new InternalState();
			XorShiftAddCore.Init(ref expected,42);
			Assert(actual, expected);

			var keys = new uint[] {42, 114514, 810};
			actual = new XorShiftAddPoolObjectPolicy(keys);
			XorShiftAddCore.Init(ref expected, keys);
			Assert(actual,expected);

			XorShiftAddCore.Init(ref expected, 114514);
			actual=new XorShiftAddPoolObjectPolicy(expected);
			Assert(actual,expected);
		}

		[Fact]
		public void CreateTest()
		{
			var policy = new XorShiftAddPoolObjectPolicy(42);
			var expected=new InternalState();
			XorShiftAddCore.Init(ref expected,42);

			var actual = policy.Create();
			





		}


	}
}
