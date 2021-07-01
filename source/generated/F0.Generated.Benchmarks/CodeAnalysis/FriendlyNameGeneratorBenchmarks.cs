using System;
using System.Collections.Generic;
using System.Numerics;
using BenchmarkDotNet.Attributes;
using F0.Extensions;
using F0.Generated;

namespace F0.Benchmarks.CodeAnalysis
{
	public class FriendlyNameGeneratorBenchmarks
	{
		private static readonly Type type = typeof(Dictionary<(BigInteger, Complex), Tuple<int[], int?[,], string[][]>>);

		[GlobalSetup]
		public void Test()
		{
			(string, string) generated = Generated();
			(string, string) reflection = Reflection();

			if (generated != reflection)
			{
				throw new InvalidOperationException();
			}
		}

		[Benchmark(Baseline = true)]
		public (string, string) Generated()
		{
			string name = Friendly.NameOf<Dictionary<(BigInteger, Complex), Tuple<int[], int?[,], string[][]>>>();
			string full = Friendly.FullNameOf<Dictionary<(BigInteger, Complex), Tuple<int[], int?[,], string[][]>>>();

			return (name, full);
		}

		[Benchmark]
		public (string, string) Reflection()
		{
			string name = type.GetFriendlyName();
			string full = type.GetFriendlyFullName();

			return (name, full);
		}
	}
}
