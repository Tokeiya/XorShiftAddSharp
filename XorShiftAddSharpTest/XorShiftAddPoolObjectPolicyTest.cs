using ChainingAssertion;
using XorShiftAddSharp;
using Xunit;
using Xunit.Abstractions;

namespace XorShiftAddSharpTest
{
	public class XorShiftAddPoolObjectPolicyTest
	{
		public XorShiftAddPoolObjectPolicyTest(ITestOutputHelper output)
		{
			_output = output;
		}

		private readonly ITestOutputHelper _output;

		private static void Assert(XorShiftAddPoolObjectPolicy actual, in InternalState expected)
		{
			var state = actual.GetCurrentState();

			for (int i = 0; i < InternalState.Size; i++) state[i].Is(expected[i]);
		}

		private static void Assert(XorShiftAdd actual, in InternalState expected)
		{
			var state = actual.GetCurrentState();

			for (int i = 0; i < InternalState.Size; i++) state[i].Is(expected[i]);
		}


		[Fact]
		public void CreateTest()
		{
			const string jump = "0x10000000000000000";
			                     

			var policy = new XorShiftAddPoolObjectPolicy(42);
			var expected = new InternalState();
			XorShiftAddCore.Init(out expected, 42);


			for (int i = 0; i < 32; i++)
			{
				var actual = policy.Create();
				Assert(actual, expected);
				XorShiftAddCore.Jump(ref expected, 1, jump);
			}
		}

		[Fact]
		public void CtorTest()
		{
			var actual = new XorShiftAddPoolObjectPolicy(42);

			InternalState expected = new InternalState();
			XorShiftAddCore.Init(out expected, 42);
			Assert(actual, expected);

			var keys = new uint[] {42, 114514, 810};
			actual = new XorShiftAddPoolObjectPolicy(keys);
			XorShiftAddCore.Init(out expected, keys);
			Assert(actual, expected);

			XorShiftAddCore.Init(out expected, 114514);
			actual = new XorShiftAddPoolObjectPolicy(expected);
			Assert(actual, expected);
		}

		[Fact]
		public void GetStateTest()
		{
			var actual = new XorShiftAddPoolObjectPolicy(42);

			InternalState expected = new InternalState();
			XorShiftAddCore.Init(out expected, 42);

			var act = actual.GetCurrentState();

			for (int i = 0; i < InternalState.Size; i++) act[i].Is(expected[i]);

			var tmp = actual.Create();
			var next = actual.GetCurrentState();
			for (int i = 0; i < InternalState.Size; i++) act[i].IsNot(next[i]);
		}

		[Fact]
		public void ReturnTest()
		{
			var policy = new XorShiftAddPoolObjectPolicy(42);

			var tmp = policy.Create();
			policy.Return(tmp).IsTrue();
		}
	}
}