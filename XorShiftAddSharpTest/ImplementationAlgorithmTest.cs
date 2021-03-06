using System.Collections.Generic;
using ChainingAssertion;
using XorShiftAddSharp;
using Xunit;
using Xunit.Abstractions;

namespace XorShiftAddSharpTest
{
	/// <summary>
	///     compromise solution :)
	/// </summary>
	public class ImplementationAlgorithmTest
	{
		public ImplementationAlgorithmTest(ITestOutputHelper output)
		{
			_output = output;
		}

		private readonly ITestOutputHelper _output;


		private static IEnumerable<int> Algorithm(IEnumerable<uint> inputs)
		{
			const uint mask = int.MaxValue;
			using var seq = inputs.GetEnumerator();

			while (seq.MoveNext())
			{
				var tmp = seq.Current;
				tmp &= mask;
				if (tmp == int.MaxValue) continue;
				yield return (int) tmp;
			}
		}


		private static IEnumerable<uint> GenerateSequence(uint seed, int length)
		{
			var state = new InternalState();
			XorShiftAddCore.Init(out state, seed);

			for (int i = 0; i < length; i++) yield return XorShiftAddCore.NextUint32(ref state);
		}


		/// <summary>
		///     Verify the Next method's algorithm and the "Algorithm" method is same one.
		/// </summary>
		[Fact]
		public void Check()
		{
			const uint seed = 42;
			var rnd = new XorShiftAdd(42);

			foreach (var i in Algorithm(GenerateSequence(seed, 1024))) rnd.Next().Is(i);
		}


		[Fact]
		public void CheckAllRange()
		{
			static IEnumerable<uint> gen()
			{
				for (uint i = 0; i < 100; i++) yield return i;

				yield return int.MaxValue;
				yield return uint.MaxValue;
				yield return uint.MinValue;
			}

			foreach (var i in Algorithm(gen())) (i >= 0 && i != int.MaxValue).IsTrue();
		}
	}
}