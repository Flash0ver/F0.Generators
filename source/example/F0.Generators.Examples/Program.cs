using System.Numerics;
using F0.Generated;

namespace F0.Generators.Examples
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			WriteLine("F0.Generators");
			WriteLine(String.Join(' ', args));

			WriteLine();

			WriteLine("# Friendly.NameOf<T>()");
			WriteLine(Friendly.NameOf<Dictionary<int, bool?>.Enumerator>());
			WriteLine(Friendly.NameOf<Tuple<string[], string[,], string[][]>>());
			WriteLine(Friendly.NameOf<(BigInteger First, Complex Second)>());

			WriteLine();

			WriteLine("# Friendly.FullNameOf<T>()");
			WriteLine(Friendly.FullNameOf<Dictionary<int, bool?>.Enumerator>());
			WriteLine(Friendly.FullNameOf<Tuple<string[], string[,], string[][]>>());
			WriteLine(Friendly.FullNameOf<(BigInteger First, Complex Second)>());

			WriteLine();

			WriteLine("# EnumInfo.GetName(Enum value)");
			WriteLine(EnumInfo.GetName(PlatformID.Other));
			WriteLine(EnumInfo.GetName(ConsoleKey.NumPad0));
			WriteLine(EnumInfo.GetName(ConsoleSpecialKey.ControlC));
			WriteLine(EnumInfo.GetName(AttributeTargets.All));
		}
	}
}
