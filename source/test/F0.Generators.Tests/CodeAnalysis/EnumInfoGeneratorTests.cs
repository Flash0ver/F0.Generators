using System;
using System.Threading.Tasks;
using F0.CodeAnalysis;
using F0.Tests.Generated;
using F0.Tests.Verifiers;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace F0.Tests.CodeAnalysis
{
	public class EnumInfoGeneratorTests
	{
		[Fact]
		public async Task Execute_Unused()
		{
			string test =
@"#nullable enable
using System;

public sealed class Class
{
	public void Method(DateTimeKind value)
	{
		_ = Enum.GetName(typeof(DateTimeKind), value);

#if (NET5_0 || !NETFRAMEWORK)
		_ = Enum.GetName<DateTimeKind>(value);
#endif

		_ = GetName();
		_ = GetName(0x_F0);
		_ = GetName(TypeCode.String);
	}

	public static string? GetName()
	{
		return null;
	}

	public static string? GetName(object value)
	{
		return value.ToString();
	}

	public static string? GetName(Enum value)
	{
		return Enum.GetName(value.GetType(), value);
	}
}
";

			string generated = CreateGenerated(null);

			await VerifyAsync(test, generated);
		}

		[Fact]
		public async Task Execute_Null()
		{
			string test =
@"#nullable enable
using System;
using F0.Generated;

public sealed class Class
{
	public void Method(Enum @enum, Enum? @null)
	{
		_ = EnumInfo.GetName(null);
		_ = EnumInfo.GetName(null!);

		_ = EnumInfo.GetName(@enum);
		_ = EnumInfo.GetName(@null!);
	}
}
";

			string generated = CreateGenerated(null);

			await VerifyAsync(test, generated);
		}

		[Fact]
		public async Task Execute_Enum()
		{
			string test =
@"#nullable enable
using System;
using F0.Generated;

public sealed class Class
{
	public void Method(StringComparison value)
	{
		_ = EnumInfo.GetName(value);
		_ = EnumInfo.GetName(value);

		_ = EnumInfo.GetName(DateTimeKind.Unspecified);
		_ = EnumInfo.GetName(DateTimeKind.Utc);
		_ = EnumInfo.GetName(DateTimeKind.Local);
		_ = EnumInfo.GetName((DateTimeKind)0);
		_ = EnumInfo.GetName((DateTimeKind)1);
		_ = EnumInfo.GetName((DateTimeKind)2);

		_ = F0.Generated.EnumInfo.GetName(UriKind.RelativeOrAbsolute);
		_ = F0.Generated.EnumInfo.GetName(UriFormat.SafeUnescaped);
		_ = F0.Generated.EnumInfo.GetName(UriPartial.Scheme);
		_ = F0.Generated.EnumInfo.GetName(UriHostNameType.IPv4);
	}
}
";

			string generated = CreateGenerated(@"
		public static string GetName(global::System.StringComparison value)
		{
			return value switch
			{
				global::System.StringComparison.CurrentCulture => nameof(global::System.StringComparison.CurrentCulture),
				global::System.StringComparison.CurrentCultureIgnoreCase => nameof(global::System.StringComparison.CurrentCultureIgnoreCase),
				global::System.StringComparison.InvariantCulture => nameof(global::System.StringComparison.InvariantCulture),
				global::System.StringComparison.InvariantCultureIgnoreCase => nameof(global::System.StringComparison.InvariantCultureIgnoreCase),
				global::System.StringComparison.Ordinal => nameof(global::System.StringComparison.Ordinal),
				global::System.StringComparison.OrdinalIgnoreCase => nameof(global::System.StringComparison.OrdinalIgnoreCase),
				_ => throw new global::System.ComponentModel.InvalidEnumArgumentException(nameof(value), (int)value, typeof(global::System.StringComparison)),
			};
		}

		public static string GetName(global::System.DateTimeKind value)
		{
			return value switch
			{
				global::System.DateTimeKind.Unspecified => nameof(global::System.DateTimeKind.Unspecified),
				global::System.DateTimeKind.Utc => nameof(global::System.DateTimeKind.Utc),
				global::System.DateTimeKind.Local => nameof(global::System.DateTimeKind.Local),
				_ => throw new global::System.ComponentModel.InvalidEnumArgumentException(nameof(value), (int)value, typeof(global::System.DateTimeKind)),
			};
		}

		public static string GetName(global::System.UriKind value)
		{
			return value switch
			{
				global::System.UriKind.RelativeOrAbsolute => nameof(global::System.UriKind.RelativeOrAbsolute),
				global::System.UriKind.Absolute => nameof(global::System.UriKind.Absolute),
				global::System.UriKind.Relative => nameof(global::System.UriKind.Relative),
				_ => throw new global::System.ComponentModel.InvalidEnumArgumentException(nameof(value), (int)value, typeof(global::System.UriKind)),
			};
		}

		public static string GetName(global::System.UriFormat value)
		{
			return value switch
			{
				global::System.UriFormat.UriEscaped => nameof(global::System.UriFormat.UriEscaped),
				global::System.UriFormat.Unescaped => nameof(global::System.UriFormat.Unescaped),
				global::System.UriFormat.SafeUnescaped => nameof(global::System.UriFormat.SafeUnescaped),
				_ => throw new global::System.ComponentModel.InvalidEnumArgumentException(nameof(value), (int)value, typeof(global::System.UriFormat)),
			};
		}

		public static string GetName(global::System.UriPartial value)
		{
			return value switch
			{
				global::System.UriPartial.Scheme => nameof(global::System.UriPartial.Scheme),
				global::System.UriPartial.Authority => nameof(global::System.UriPartial.Authority),
				global::System.UriPartial.Path => nameof(global::System.UriPartial.Path),
				global::System.UriPartial.Query => nameof(global::System.UriPartial.Query),
				_ => throw new global::System.ComponentModel.InvalidEnumArgumentException(nameof(value), (int)value, typeof(global::System.UriPartial)),
			};
		}

		public static string GetName(global::System.UriHostNameType value)
		{
			return value switch
			{
				global::System.UriHostNameType.Unknown => nameof(global::System.UriHostNameType.Unknown),
				global::System.UriHostNameType.Basic => nameof(global::System.UriHostNameType.Basic),
				global::System.UriHostNameType.Dns => nameof(global::System.UriHostNameType.Dns),
				global::System.UriHostNameType.IPv4 => nameof(global::System.UriHostNameType.IPv4),
				global::System.UriHostNameType.IPv6 => nameof(global::System.UriHostNameType.IPv6),
				_ => throw new global::System.ComponentModel.InvalidEnumArgumentException(nameof(value), (int)value, typeof(global::System.UriHostNameType)),
			};
		}");

			await VerifyAsync(test, generated);
		}

		[Fact]
		public async Task Execute_Flags()
		{
			string test =
@"#nullable enable
using System;
using System.IO;
using System.Reflection;
using F0.Generated;

public sealed class Class
{
	public void Method(StringSplitOptions value)
	{
		_ = EnumInfo.GetName(value);
		_ = EnumInfo.GetName(value);

		_ = EnumInfo.GetName(ConsoleModifiers.Alt);
		_ = EnumInfo.GetName(ConsoleModifiers.Alt | ConsoleModifiers.Shift);
		_ = EnumInfo.GetName(ConsoleModifiers.Alt | ConsoleModifiers.Shift | ConsoleModifiers.Control);
		_ = EnumInfo.GetName(ConsoleModifiers.Alt & ConsoleModifiers.Shift & ConsoleModifiers.Control);
		_ = EnumInfo.GetName(ConsoleModifiers.Alt ^ ConsoleModifiers.Shift ^ ConsoleModifiers.Control);
		_ = EnumInfo.GetName(~ConsoleModifiers.Alt);

		_ = EnumInfo.GetName(FileAccess.Read);
		_ = EnumInfo.GetName(System.IO.FileAccess.Write);

		_ = F0.Generated.EnumInfo.GetName(ResourceLocation.Embedded);
		_ = F0.Generated.EnumInfo.GetName(ResourceLocation.ContainedInAnotherAssembly);
		_ = F0.Generated.EnumInfo.GetName(ResourceLocation.ContainedInManifestFile);
	}
}
";

			string generated = CreateGenerated(@"
		public static string GetName(global::System.StringSplitOptions value)
		{
			throw new global::F0.Generated.SourceGenerationException(""Flags are not yet supported: see https://github.com/Flash0ver/F0.Generators/issues/1"");
		}

		public static string GetName(global::System.ConsoleModifiers value)
		{
			throw new global::F0.Generated.SourceGenerationException(""Flags are not yet supported: see https://github.com/Flash0ver/F0.Generators/issues/1"");
		}

		public static string GetName(global::System.IO.FileAccess value)
		{
			throw new global::F0.Generated.SourceGenerationException(""Flags are not yet supported: see https://github.com/Flash0ver/F0.Generators/issues/1"");
		}

		public static string GetName(global::System.Reflection.ResourceLocation value)
		{
			throw new global::F0.Generated.SourceGenerationException(""Flags are not yet supported: see https://github.com/Flash0ver/F0.Generators/issues/1"");
		}");

			await VerifyAsync(test, generated);
		}

		[Fact]
		public async Task Execute_LanguageVersion_CSharp7_3()
		{
			string test =
@"using System;
using System.Threading.Tasks;
using F0.Generated;

public sealed class Class
{
	public void Method(TaskStatus value)
	{
		_ = EnumInfo.GetName(value);
	}
}
";

			string generated = CreateGenerated(@"
		public static string GetName(global::System.Threading.Tasks.TaskStatus value)
		{
			switch (value)
			{
				case global::System.Threading.Tasks.TaskStatus.Created:
					return nameof(global::System.Threading.Tasks.TaskStatus.Created);
				case global::System.Threading.Tasks.TaskStatus.WaitingForActivation:
					return nameof(global::System.Threading.Tasks.TaskStatus.WaitingForActivation);
				case global::System.Threading.Tasks.TaskStatus.WaitingToRun:
					return nameof(global::System.Threading.Tasks.TaskStatus.WaitingToRun);
				case global::System.Threading.Tasks.TaskStatus.Running:
					return nameof(global::System.Threading.Tasks.TaskStatus.Running);
				case global::System.Threading.Tasks.TaskStatus.WaitingForChildrenToComplete:
					return nameof(global::System.Threading.Tasks.TaskStatus.WaitingForChildrenToComplete);
				case global::System.Threading.Tasks.TaskStatus.RanToCompletion:
					return nameof(global::System.Threading.Tasks.TaskStatus.RanToCompletion);
				case global::System.Threading.Tasks.TaskStatus.Canceled:
					return nameof(global::System.Threading.Tasks.TaskStatus.Canceled);
				case global::System.Threading.Tasks.TaskStatus.Faulted:
					return nameof(global::System.Threading.Tasks.TaskStatus.Faulted);
				default:
					throw new global::System.ComponentModel.InvalidEnumArgumentException(nameof(value), (int)value, typeof(global::System.Threading.Tasks.TaskStatus));
			}
		}", LanguageVersion.CSharp7_3);

			await VerifyAsync(test, generated, LanguageVersion.CSharp7_3);
		}

		[Fact]
		public async Task Execute_LanguageVersion_CSharp5()
		{
			string test =
@"using System;
using System.Diagnostics;
using F0.Generated;

public sealed class Class
{
	public void Method(DebuggerBrowsableState value)
	{
		string unused = EnumInfo.GetName(value);
	}
}
";

			string generated = CreateGenerated(@"
		public static string GetName(global::System.Diagnostics.DebuggerBrowsableState value)
		{
			switch (value)
			{
				case global::System.Diagnostics.DebuggerBrowsableState.Never:
					return ""Never"";
				case global::System.Diagnostics.DebuggerBrowsableState.Collapsed:
					return ""Collapsed"";
				case global::System.Diagnostics.DebuggerBrowsableState.RootHidden:
					return ""RootHidden"";
				default:
					throw new global::System.ComponentModel.InvalidEnumArgumentException(""value"", (int)value, typeof(global::System.Diagnostics.DebuggerBrowsableState));
			}
		}", LanguageVersion.CSharp5);

			await VerifyAsync(test, generated, LanguageVersion.CSharp5);
		}

		[Fact]
		public async Task Execute_LanguageVersion_CSharp1()
		{
			string test =
@"using System;
using F0.Generated;

public sealed class Class
{
	public void Method(MidpointRounding value)
	{
		string unused = EnumInfo.GetName(value);
	}
}
";

			string generated = CreateGenerated(@"
		public static string GetName(System.MidpointRounding value)
		{
			switch (value)
			{
				case System.MidpointRounding.ToEven:
					return ""ToEven"";
				case System.MidpointRounding.AwayFromZero:
					return ""AwayFromZero"";
				case System.MidpointRounding.ToZero:
					return ""ToZero"";
				case System.MidpointRounding.ToNegativeInfinity:
					return ""ToNegativeInfinity"";
				case System.MidpointRounding.ToPositiveInfinity:
					return ""ToPositiveInfinity"";
				default:
					throw new System.ComponentModel.InvalidEnumArgumentException(""value"", (int)value, typeof(System.MidpointRounding));
			}
		}", LanguageVersion.CSharp1);

			await VerifyAsync(test, generated, LanguageVersion.CSharp1);
		}

		private static string CreateGenerated(string? code, LanguageVersion? languageVersion = null)
		{
			string source = code is null ? String.Empty : Environment.NewLine + code;
			LanguageVersion version = languageVersion.GetValueOrDefault(LanguageVersion.Latest);

			string classDeclaration = version switch
			{
				>= LanguageVersion.CSharp2 => "internal static class EnumInfo",
				_ => "internal class EnumInfo",
			};

			string constructorDeclaration = version switch
			{
				>= LanguageVersion.CSharp2 => String.Empty,
				_ => @"
		private EnumInfo()
		{
		}
",
			};

			string methodDeclaration = version switch
			{
				>= LanguageVersion.CSharp8 => "public static string GetName(global::System.Enum? value)",
				>= LanguageVersion.CSharp2 => "public static string GetName(global::System.Enum value)",
				_ => "public static string GetName(System.Enum value)",
			};

			string throwStatement = version switch
			{
				>= LanguageVersion.CSharp6 => $@"throw new global::F0.Generated.SourceGenerationException($""Cannot use the unspecialized method, which serves as a placeholder for the generator. Enum-Type {{value?.GetType().ToString() ?? ""<null>""}} must be concrete to generate the allocation-free variant of {nameof(Enum)}.{nameof(Enum.ToString)}()."");",
				>= LanguageVersion.CSharp2 => $@"throw new global::F0.Generated.SourceGenerationException(""Cannot use the unspecialized method, which serves as a placeholder for the generator. Enum-Type "" + (value == null ? ""<null>"" : value.GetType().ToString()) + "" must be concrete to generate the allocation-free variant of {nameof(Enum)}.{nameof(Enum.ToString)}()."");",
				_ => $@"throw new F0.Generated.SourceGenerationException(""Cannot use the unspecialized method, which serves as a placeholder for the generator. Enum-Type "" + (value == null ? ""<null>"" : value.GetType().ToString()) + "" must be concrete to generate the allocation-free variant of {nameof(Enum)}.{nameof(Enum.ToString)}()."");",
			};

			return $@"namespace F0.Generated
{{
	{classDeclaration}
	{{{constructorDeclaration}
		{methodDeclaration}
		{{
			{throwStatement}
		}}{source}
	}}
}}
";
		}

		private static Task VerifyAsync(string test, string generated, LanguageVersion? languageVersion = null)
		{
			string filename = $@"F0.Generators\{typeof(EnumInfoGenerator).FullName}\EnumInfo.g.cs";
			string content = String.Concat(Sources.GetFileHeader(languageVersion), generated);

			test += Sources.SourceGenerationException_String;

			return CSharpSourceGeneratorVerifier<EnumInfoGenerator>.VerifySourceGeneratorAsync(test, (filename, content), languageVersion, ReferenceAssemblies.Net.Net50);
		}
	}
}
