using System;
using System.Collections.Generic;
using System.Numerics;
using F0.Generated;

namespace F0.Generators.Examples
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			Console.WriteLine("F0.Generators");
			Console.WriteLine(String.Join(' ', args));

			Console.WriteLine();

			Console.WriteLine("# Friendly.NameOf<T>()");
			Console.WriteLine(Friendly.NameOf<Dictionary<int, bool?>.Enumerator>());
			Console.WriteLine(Friendly.NameOf<Tuple<string[], string[,], string[][]>>());
			Console.WriteLine(Friendly.NameOf<(BigInteger First, Complex Second)>());

			Console.WriteLine();

			Console.WriteLine("# Friendly.FullNameOf<T>()");
			Console.WriteLine(Friendly.FullNameOf<Dictionary<int, bool?>.Enumerator>());
			Console.WriteLine(Friendly.FullNameOf<Tuple<string[], string[,], string[][]>>());
			Console.WriteLine(Friendly.FullNameOf<(BigInteger First, Complex Second)>());
		}
	}
}
