using System;
using System.Threading.Tasks;
using F0.CodeAnalysis;
using F0.Tests.Generated;
using F0.Tests.Verifiers;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace F0.Tests.CodeAnalysis
{
	public class FriendlyNameGeneratorTests
	{
		[Fact]
		public async Task Execute_Unused()
		{
			string test =
@"#nullable enable
using System;

public sealed class Class
{
	public void Method()
	{
		_ = nameof(Class);
		_ = typeof(Class).Name;
		_ = typeof(Class).FullName;
	}
}
" + Sources.SourceGenerationException;

			string generated =
@"namespace F0.Generated
{
	internal static class Friendly
	{
		public static string NameOf<T>()
		{
			throw new global::F0.Generated.SourceGenerationException();
		}

		public static string FullNameOf<T>()
		{
			throw new global::F0.Generated.SourceGenerationException();
		}
	}
}
";

			await VerifyAsync(test, generated);
		}

		[Fact]
		public async Task Execute_PredefinedType()
		{
			string test =
@"#nullable enable
using System;

public sealed class Class
{
	public void Method()
	{
		_ = F0.Generated.Friendly.NameOf<int>();
		_ = F0.Generated.Friendly.NameOf<string>();

		_ = F0.Generated.Friendly.FullNameOf<int>();
		_ = F0.Generated.Friendly.FullNameOf<string>();
	}
}
";

			string generated =
@"namespace F0.Generated
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
			return new(2)
			{
				{ typeof(int), ""int"" },
				{ typeof(string), ""string"" },
			};
		}

		private static global::System.Collections.Generic.Dictionary<global::System.Type, string> CreateFullNameOfLookup()
		{
			return new(2)
			{
				{ typeof(int), ""int"" },
				{ typeof(string), ""string"" },
			};
		}
	}
}
";

			await VerifyAsync(test, generated);
		}

		[Fact]
		public async Task Execute_NullableType()
		{
			string test =
@"#nullable enable
using System;
using System.Numerics;

public sealed class Class
{
	public void Method()
	{
		_ = F0.Generated.Friendly.NameOf<int?>();
		_ = F0.Generated.Friendly.NameOf<string?>();
		_ = F0.Generated.Friendly.NameOf<BigInteger?>();
		_ = F0.Generated.Friendly.NameOf<Type?>();

		_ = F0.Generated.Friendly.FullNameOf<int?>();
		_ = F0.Generated.Friendly.FullNameOf<string?>();
		_ = F0.Generated.Friendly.FullNameOf<BigInteger?>();
		_ = F0.Generated.Friendly.FullNameOf<Type?>();
	}
}
";

			string generated =
@"namespace F0.Generated
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
			return new(4)
			{
				{ typeof(int?), ""int?"" },
				{ typeof(string), ""string"" },
				{ typeof(global::System.Numerics.BigInteger?), ""BigInteger?"" },
				{ typeof(global::System.Type), ""Type"" },
			};
		}

		private static global::System.Collections.Generic.Dictionary<global::System.Type, string> CreateFullNameOfLookup()
		{
			return new(4)
			{
				{ typeof(int?), ""int?"" },
				{ typeof(string), ""string"" },
				{ typeof(global::System.Numerics.BigInteger?), ""System.Numerics.BigInteger?"" },
				{ typeof(global::System.Type), ""System.Type"" },
			};
		}
	}
}
";

			await VerifyAsync(test, generated);
		}

		[Fact]
		public async Task Execute_ArrayType()
		{
			string test =
@"#nullable enable
using System;
using System.Threading.Tasks;

public sealed class Class
{
	public void Method()
	{
		_ = F0.Generated.Friendly.NameOf<int[]>();
		_ = F0.Generated.Friendly.NameOf<Task[]>();
		_ = F0.Generated.Friendly.NameOf<int[,]>();
		_ = F0.Generated.Friendly.NameOf<Task[,]>();
		_ = F0.Generated.Friendly.NameOf<int[][]>();
		_ = F0.Generated.Friendly.NameOf<Task[][]>();

		_ = F0.Generated.Friendly.FullNameOf<int[]>();
		_ = F0.Generated.Friendly.FullNameOf<Task[]>();
		_ = F0.Generated.Friendly.FullNameOf<int[,]>();
		_ = F0.Generated.Friendly.FullNameOf<Task[,]>();
		_ = F0.Generated.Friendly.FullNameOf<int[][]>();
		_ = F0.Generated.Friendly.FullNameOf<Task[][]>();
	}
}
";

			string generated =
@"namespace F0.Generated
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
			return new(6)
			{
				{ typeof(int[]), ""int[]"" },
				{ typeof(global::System.Threading.Tasks.Task[]), ""Task[]"" },
				{ typeof(int[,]), ""int[,]"" },
				{ typeof(global::System.Threading.Tasks.Task[,]), ""Task[,]"" },
				{ typeof(int[][]), ""int[][]"" },
				{ typeof(global::System.Threading.Tasks.Task[][]), ""Task[][]"" },
			};
		}

		private static global::System.Collections.Generic.Dictionary<global::System.Type, string> CreateFullNameOfLookup()
		{
			return new(6)
			{
				{ typeof(int[]), ""int[]"" },
				{ typeof(global::System.Threading.Tasks.Task[]), ""System.Threading.Tasks.Task[]"" },
				{ typeof(int[,]), ""int[,]"" },
				{ typeof(global::System.Threading.Tasks.Task[,]), ""System.Threading.Tasks.Task[,]"" },
				{ typeof(int[][]), ""int[][]"" },
				{ typeof(global::System.Threading.Tasks.Task[][]), ""System.Threading.Tasks.Task[][]"" },
			};
		}
	}
}
";

			await VerifyAsync(test, generated);
		}

		[Fact]
		public async Task Execute_IdentifierName()
		{
			string test =
@"#nullable enable
using System;
using System.Numerics;

public sealed class Class
{
	public void Method()
	{
		_ = F0.Generated.Friendly.NameOf<BigInteger>();
		_ = F0.Generated.Friendly.NameOf<Type>();

		_ = F0.Generated.Friendly.FullNameOf<BigInteger>();
		_ = F0.Generated.Friendly.FullNameOf<Type>();
	}
}
";

			string generated =
@"namespace F0.Generated
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
			return new(2)
			{
				{ typeof(global::System.Numerics.BigInteger), ""BigInteger"" },
				{ typeof(global::System.Type), ""Type"" },
			};
		}

		private static global::System.Collections.Generic.Dictionary<global::System.Type, string> CreateFullNameOfLookup()
		{
			return new(2)
			{
				{ typeof(global::System.Numerics.BigInteger), ""System.Numerics.BigInteger"" },
				{ typeof(global::System.Type), ""System.Type"" },
			};
		}
	}
}
";

			await VerifyAsync(test, generated);
		}

		[Fact]
		public async Task Execute_QualifiedName()
		{
			string test =
@"#nullable enable
using System;
using System.Collections.Generic;

public sealed class Class
{
	public void Method()
	{
		_ = F0.Generated.Friendly.NameOf<List<Type>.Enumerator>();

		_ = F0.Generated.Friendly.FullNameOf<List<Type>.Enumerator>();
	}
}
";

			string generated =
@"namespace F0.Generated
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
			return new(1)
			{
				{ typeof(global::System.Collections.Generic.List<global::System.Type>.Enumerator), ""List<Type>.Enumerator"" },
			};
		}

		private static global::System.Collections.Generic.Dictionary<global::System.Type, string> CreateFullNameOfLookup()
		{
			return new(1)
			{
				{ typeof(global::System.Collections.Generic.List<global::System.Type>.Enumerator), ""System.Collections.Generic.List<System.Type>.Enumerator"" },
			};
		}
	}
}
";

			await VerifyAsync(test, generated);
		}

		[Fact]
		public async Task Execute_GenericName()
		{
			string test =
@"#nullable enable
using System;
using System.Collections.Generic;
using System.Numerics;

public sealed class Class
{
	public void Method()
	{
		_ = F0.Generated.Friendly.NameOf<IEquatable<int>>();
		_ = F0.Generated.Friendly.NameOf<IEnumerable<Type>>();
		_ = F0.Generated.Friendly.NameOf<Dictionary<Complex, Complex?>>();
		_ = F0.Generated.Friendly.NameOf<Tuple<int, string>>();
		_ = F0.Generated.Friendly.NameOf<Tuple<DateTime, DateTimeOffset, DateTimeKind>>();

		_ = F0.Generated.Friendly.FullNameOf<IEquatable<int>>();
		_ = F0.Generated.Friendly.FullNameOf<IEnumerable<Type>>();
		_ = F0.Generated.Friendly.FullNameOf<Dictionary<Complex, Complex?>>();
		_ = F0.Generated.Friendly.FullNameOf<Tuple<int, string>>();
		_ = F0.Generated.Friendly.FullNameOf<Tuple<DateTime, DateTimeOffset, DateTimeKind>>();
	}
}
";

			string generated =
@"namespace F0.Generated
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
			return new(5)
			{
				{ typeof(global::System.IEquatable<int>), ""IEquatable<int>"" },
				{ typeof(global::System.Collections.Generic.IEnumerable<global::System.Type>), ""IEnumerable<Type>"" },
				{ typeof(global::System.Collections.Generic.Dictionary<global::System.Numerics.Complex, global::System.Numerics.Complex?>), ""Dictionary<Complex, Complex?>"" },
				{ typeof(global::System.Tuple<int, string>), ""Tuple<int, string>"" },
				{ typeof(global::System.Tuple<global::System.DateTime, global::System.DateTimeOffset, global::System.DateTimeKind>), ""Tuple<DateTime, DateTimeOffset, DateTimeKind>"" },
			};
		}

		private static global::System.Collections.Generic.Dictionary<global::System.Type, string> CreateFullNameOfLookup()
		{
			return new(5)
			{
				{ typeof(global::System.IEquatable<int>), ""System.IEquatable<int>"" },
				{ typeof(global::System.Collections.Generic.IEnumerable<global::System.Type>), ""System.Collections.Generic.IEnumerable<System.Type>"" },
				{ typeof(global::System.Collections.Generic.Dictionary<global::System.Numerics.Complex, global::System.Numerics.Complex?>), ""System.Collections.Generic.Dictionary<System.Numerics.Complex, System.Numerics.Complex?>"" },
				{ typeof(global::System.Tuple<int, string>), ""System.Tuple<int, string>"" },
				{ typeof(global::System.Tuple<global::System.DateTime, global::System.DateTimeOffset, global::System.DateTimeKind>), ""System.Tuple<System.DateTime, System.DateTimeOffset, System.DateTimeKind>"" },
			};
		}
	}
}
";

			await VerifyAsync(test, generated);
		}

		[Fact]
		public async Task Execute_TupleType()
		{
			string test =
@"#nullable enable
using System;
using System.Numerics;

public sealed class Class
{
	public void Method()
	{
		_ = F0.Generated.Friendly.NameOf<(int Number, string Text)>();
		_ = F0.Generated.Friendly.NameOf<(int, string)>();
		_ = F0.Generated.Friendly.NameOf<(BigInteger Item1, Complex Item2)>();
		_ = F0.Generated.Friendly.NameOf<(BigInteger, Complex)>();

		_ = F0.Generated.Friendly.FullNameOf<(int Number, string Text)>();
		_ = F0.Generated.Friendly.FullNameOf<(int, string)>();
		_ = F0.Generated.Friendly.FullNameOf<(BigInteger Item1, Complex Item2)>();
		_ = F0.Generated.Friendly.FullNameOf<(BigInteger, Complex)>();
	}
}
";

			string generated =
@"namespace F0.Generated
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
			return new(2)
			{
				{ typeof((int, string)), ""(int, string)"" },
				{ typeof((global::System.Numerics.BigInteger, global::System.Numerics.Complex)), ""(BigInteger, Complex)"" },
			};
		}

		private static global::System.Collections.Generic.Dictionary<global::System.Type, string> CreateFullNameOfLookup()
		{
			return new(2)
			{
				{ typeof((int, string)), ""(int, string)"" },
				{ typeof((global::System.Numerics.BigInteger, global::System.Numerics.Complex)), ""(System.Numerics.BigInteger, System.Numerics.Complex)"" },
			};
		}
	}
}
";

			await VerifyAsync(test, generated);
		}

		[Fact]
		public async Task Execute_LanguageVersion_CSharp8()
		{
			string test =
@"#nullable enable
using System;

public sealed class Class
{
	public void Method()
	{
		_ = F0.Generated.Friendly.NameOf<Object>();

		_ = F0.Generated.Friendly.FullNameOf<Object>();
	}
}
";

			string generated =
@"namespace F0.Generated
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
			return new global::System.Collections.Generic.Dictionary<global::System.Type, string>(1)
			{
				{ typeof(object), ""object"" },
			};
		}

		private static global::System.Collections.Generic.Dictionary<global::System.Type, string> CreateFullNameOfLookup()
		{
			return new global::System.Collections.Generic.Dictionary<global::System.Type, string>(1)
			{
				{ typeof(object), ""object"" },
			};
		}
	}
}
";

			await VerifyAsync(test, generated, LanguageVersion.CSharp8);
		}

		[Fact]
		public async Task Execute_LanguageVersion_CSharp7_3()
		{
			string test =
@"using System;

public sealed class Class
{
	public void Method()
	{
		_ = F0.Generated.Friendly.NameOf<Object>();

		_ = F0.Generated.Friendly.FullNameOf<Object>();
	}
}
";

			string generated =
@"namespace F0.Generated
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
			return new global::System.Collections.Generic.Dictionary<global::System.Type, string>(1)
			{
				{ typeof(object), ""object"" },
			};
		}

		private static global::System.Collections.Generic.Dictionary<global::System.Type, string> CreateFullNameOfLookup()
		{
			return new global::System.Collections.Generic.Dictionary<global::System.Type, string>(1)
			{
				{ typeof(object), ""object"" },
			};
		}
	}
}
";

			await VerifyAsync(test, generated, LanguageVersion.CSharp7_3);
		}

		[Fact]
		public async Task Execute_LanguageVersion_CSharp2()
		{
			string test =
@"using System;

public sealed class Class
{
	public void Method()
	{
		string unused = F0.Generated.Friendly.NameOf<Object>();

		unused = F0.Generated.Friendly.FullNameOf<Object>();
	}
}
" + Sources.SourceGenerationException_String;

			string generated =
@"namespace F0.Generated
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
			global::System.Collections.Generic.Dictionary<global::System.Type, string> dictionary = new global::System.Collections.Generic.Dictionary<global::System.Type, string>(1);
			dictionary.Add(typeof(object), ""object"");
			return dictionary;
		}

		private static global::System.Collections.Generic.Dictionary<global::System.Type, string> CreateFullNameOfLookup()
		{
			global::System.Collections.Generic.Dictionary<global::System.Type, string> dictionary = new global::System.Collections.Generic.Dictionary<global::System.Type, string>(1);
			dictionary.Add(typeof(object), ""object"");
			return dictionary;
		}
	}
}
";

			await VerifyAsync(test, generated, LanguageVersion.CSharp2);
		}

		[Fact]
		public async Task Execute_LanguageVersion_CSharp1()
		{
			string test =
@"using System;

public sealed class Class
{
	public void Method()
	{
		string unused = F0.Generated.Friendly.NameOf();

		unused = F0.Generated.Friendly.FullNameOf();
	}
}
" + Sources.SourceGenerationException_String;

			string generated =
@"namespace F0.Generated
{
	internal class Friendly
	{
		public static string NameOf()
		{
			throw new F0.Generated.SourceGenerationException(""Feature is not available in C# 1. Please use language version 2 or greater."");
		}

		public static string FullNameOf()
		{
			throw new F0.Generated.SourceGenerationException(""Feature is not available in C# 1. Please use language version 2 or greater."");
		}

		private Friendly()
		{
			throw new F0.Generated.SourceGenerationException(""Feature is not available in C# 1. Please use language version 2 or greater."");
		}
	}
}
";

			await VerifyAsync(test, generated, LanguageVersion.CSharp1);
		}

		private static Task VerifyAsync(string test, string generated, LanguageVersion? languageVersion = null)
		{
			string filename = $@"F0.Generators\{typeof(FriendlyNameGenerator).FullName}\Friendly.g.cs";
			string content = String.Concat(Sources.GetFileHeader(languageVersion), generated);

#if NETFRAMEWORK
			return CSharpSourceGeneratorVerifier<FriendlyNameGenerator>.VerifySourceGeneratorAsync(test, (filename, content), languageVersion, Microsoft.CodeAnalysis.Testing.ReferenceAssemblies.NetStandard.NetStandard20);
#else
			return CSharpSourceGeneratorVerifier<FriendlyNameGenerator>.VerifySourceGeneratorAsync(test, (filename, content), languageVersion);
#endif
		}
	}
}
