using System;
using BenchmarkDotNet.Attributes;
using F0.Generated;
using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace F0.Benchmarks.CodeAnalysis
{
	public class EnumInfoGeneratorBenchmarks
	{
		private const LogLevel logLevel = LogLevel.None;

		[GlobalSetup]
		public void Test()
		{
			Generated().Should().Be(nameof(LogLevel.None));
			Enum_ToString().Should().Be(nameof(LogLevel.None));
			Enum_GetName().Should().Be(nameof(LogLevel.None));
		}

		[Benchmark(Baseline = true)]
		public string Generated()
		{
			return EnumInfo.GetName(logLevel);
		}

		[Benchmark]
		public string Enum_ToString()
		{
			return logLevel.ToString();
		}

		[Benchmark]
		public string? Enum_GetName()
		{
			return Enum.GetName(logLevel);
		}
	}
}
