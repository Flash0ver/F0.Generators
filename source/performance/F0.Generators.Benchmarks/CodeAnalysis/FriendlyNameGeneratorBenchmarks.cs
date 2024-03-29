using F0.Benchmarks.Measurers;
using F0.CodeAnalysis;

namespace F0.Benchmarks.CodeAnalysis;

public class FriendlyNameGeneratorBenchmarks
{
	private readonly CSharpSourceGeneratorMeasurer<FriendlyNameGenerator> benchmark = new();

	[GlobalSetup]
	public void Setup()
	{
		string code = """
			#nullable enable
			using System;
			using System.Collections.Generic;

			public sealed class Class
			{
				public void Method()
				{
					_ = F0.Generated.Friendly.NameOf<int>();
					_ = F0.Generated.Friendly.NameOf<int?>();
					_ = F0.Generated.Friendly.NameOf<string>();
					_ = F0.Generated.Friendly.NameOf<string?>();
					_ = F0.Generated.Friendly.NameOf<Tuple<int[], int[,], int[][]>>();
					_ = F0.Generated.Friendly.NameOf<Dictionary<Type, Type>.Enumerator>();
					_ = F0.Generated.Friendly.NameOf<(DateTime First, DateTimeOffset Second)>();

					_ = F0.Generated.Friendly.FullNameOf<int>();
					_ = F0.Generated.Friendly.FullNameOf<int?>();
					_ = F0.Generated.Friendly.FullNameOf<string>();
					_ = F0.Generated.Friendly.FullNameOf<string?>();
					_ = F0.Generated.Friendly.FullNameOf<Tuple<int[], int[,], int[][]>>();
					_ = F0.Generated.Friendly.FullNameOf<Dictionary<Type, Type>.Enumerator>();
					_ = F0.Generated.Friendly.FullNameOf<(DateTime First, DateTimeOffset Second)>();
				}
			}
			""";

		benchmark.Initialize(code);
	}

	[Benchmark]
	public object? Generate()
	{
		benchmark.Invoke();
		return null;
	}

	[GlobalCleanup]
	public void Cleanup()
	{
		const int capacity = 6;

		string generated = $$"""
			// <auto-generated/>

			#nullable enable

			namespace F0.Generated
			{
				internal static class Friendly
				{
					private static readonly global::System.Collections.Generic.Dictionary<global::System.Type, string> nameOf = CreateNameOfLookup();
					private static readonly global::System.Collections.Generic.Dictionary<global::System.Type, string> fullNameOf = CreateFullNameOfLookup();

					public static string NameOf<T>()
					{
						return nameOf[typeof(T)];
					}

					public static string FullNameOf<T>()
					{
						return fullNameOf[typeof(T)];
					}

					private static global::System.Collections.Generic.Dictionary<global::System.Type, string> CreateNameOfLookup()
					{
						return new({{capacity}})
						{
							{ typeof(int), "int" },
							{ typeof(int?), "int?" },
							{ typeof(string), "string" },
							{ typeof(global::System.Tuple<int[], int[,], int[][]>), "Tuple<int[], int[,], int[][]>" },
							{ typeof(global::System.Collections.Generic.Dictionary<global::System.Type, global::System.Type>.Enumerator), "Dictionary<Type, Type>.Enumerator" },
							{ typeof((global::System.DateTime, global::System.DateTimeOffset)), "(DateTime, DateTimeOffset)" },
						};
					}

					private static global::System.Collections.Generic.Dictionary<global::System.Type, string> CreateFullNameOfLookup()
					{
						return new({{capacity}})
						{
							{ typeof(int), "int" },
							{ typeof(int?), "int?" },
							{ typeof(string), "string" },
							{ typeof(global::System.Tuple<int[], int[,], int[][]>), "System.Tuple<int[], int[,], int[][]>" },
							{ typeof(global::System.Collections.Generic.Dictionary<global::System.Type, global::System.Type>.Enumerator), "System.Collections.Generic.Dictionary<System.Type, System.Type>.Enumerator" },
							{ typeof((global::System.DateTime, global::System.DateTimeOffset)), "(System.DateTime, System.DateTimeOffset)" },
						};
					}
				}
			}

			""";

		benchmark.Inspect(generated);
	}
}
