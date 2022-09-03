using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using F0.CodeAnalysis;
using F0.Diagnostics;
using F0.Tests.CSharp;
using F0.Tests.Generated;
using F0.Tests.Verifiers;

namespace F0.Tests.CodeAnalysis;

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
	public async Task Execute_Error()
	{
		string test =
@"#nullable enable
using System;
using System.IO;
using System.Net.Sockets;
using F0.Generated;

public sealed class Class
{
	public void Method(Enum @enum)
	{
		_ = EnumInfo.{|#0:GetName|}();
		_ = EnumInfo.GetName({|#1:notExisting|});
		_ = EnumInfo.GetName({|#2:0x_F0|});
		_ = EnumInfo.GetName({|#3:""0x_F0""|});
		_ = EnumInfo.GetName({|#4:SocketFlags|});
		_ = EnumInfo.GetName({|#5:System|});
		_ = EnumInfo.GetName(FileAttributes.{|#6:|});
		_ = EnumInfo.GetName(UriComponents.{|#7:NotDefined|});
	}
}
";

		DiagnosticResult[] diagnostics = new[]
		{
			CreateDiagnostic("CS1501", DiagnosticSeverity.Error).WithLocation(0),
			CreateDiagnostic("CS0103", DiagnosticSeverity.Error).WithLocation(1),
			CreateDiagnostic("CS1503", DiagnosticSeverity.Error).WithLocation(2),
			CreateDiagnostic("CS1503", DiagnosticSeverity.Error).WithLocation(3),
			CreateDiagnostic("CS0119", DiagnosticSeverity.Error).WithLocation(4),
			CreateDiagnostic("CS0118", DiagnosticSeverity.Error).WithLocation(5),
			CreateDiagnostic("CS0117", DiagnosticSeverity.Error).WithLocation(6),
			CreateDiagnostic("CS1001", DiagnosticSeverity.Error).WithSpan(17, 39, 17, 40),
			CreateDiagnostic("CS0117", DiagnosticSeverity.Error).WithLocation(7),
		};

		string generated = CreateGenerated(@"
		public static string? GetName(global::System.Net.Sockets.SocketFlags value)
		{
			return value switch
			{
				global::System.Net.Sockets.SocketFlags.None => nameof(global::System.Net.Sockets.SocketFlags.None),
				global::System.Net.Sockets.SocketFlags.OutOfBand => nameof(global::System.Net.Sockets.SocketFlags.OutOfBand),
				global::System.Net.Sockets.SocketFlags.Peek => nameof(global::System.Net.Sockets.SocketFlags.Peek),
				global::System.Net.Sockets.SocketFlags.DontRoute => nameof(global::System.Net.Sockets.SocketFlags.DontRoute),
				global::System.Net.Sockets.SocketFlags.Truncated => nameof(global::System.Net.Sockets.SocketFlags.Truncated),
				global::System.Net.Sockets.SocketFlags.ControlDataTruncated => nameof(global::System.Net.Sockets.SocketFlags.ControlDataTruncated),
				global::System.Net.Sockets.SocketFlags.Broadcast => nameof(global::System.Net.Sockets.SocketFlags.Broadcast),
				global::System.Net.Sockets.SocketFlags.Multicast => nameof(global::System.Net.Sockets.SocketFlags.Multicast),
				global::System.Net.Sockets.SocketFlags.Partial => nameof(global::System.Net.Sockets.SocketFlags.Partial),
				_ => null,
			};
		}");

		await VerifyAsync(test, diagnostics, generated);
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
		_ = {|#0:EnumInfo.GetName(null)|};
		_ = {|#1:EnumInfo.GetName(null!)|};

		_ = {|#2:EnumInfo.GetName(@enum)|};
		_ = {|#3:EnumInfo.GetName(@null!)|};

		_ = {|#4:F0.Generated.EnumInfo.GetName(null)|}!.Length;
	}
}
";

		DiagnosticResult[] diagnostics = new[]
		{
			CreateUnspecializedPlaceholderDiagnostic(0, "null"),
			CreateUnspecializedPlaceholderDiagnostic(1, "null"),
			CreateUnspecializedPlaceholderDiagnostic(2, "Enum"),
			CreateUnspecializedPlaceholderDiagnostic(3, "Enum"),
			CreateUnspecializedPlaceholderDiagnostic(4, "null"),
		};

		string generated = CreateGenerated(null);

		await VerifyAsync(test, diagnostics, generated);
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
		public static string? GetName(global::System.StringComparison value)
		{
			return value switch
			{
				global::System.StringComparison.CurrentCulture => nameof(global::System.StringComparison.CurrentCulture),
				global::System.StringComparison.CurrentCultureIgnoreCase => nameof(global::System.StringComparison.CurrentCultureIgnoreCase),
				global::System.StringComparison.InvariantCulture => nameof(global::System.StringComparison.InvariantCulture),
				global::System.StringComparison.InvariantCultureIgnoreCase => nameof(global::System.StringComparison.InvariantCultureIgnoreCase),
				global::System.StringComparison.Ordinal => nameof(global::System.StringComparison.Ordinal),
				global::System.StringComparison.OrdinalIgnoreCase => nameof(global::System.StringComparison.OrdinalIgnoreCase),
				_ => null,
			};
		}

		public static string? GetName(global::System.DateTimeKind value)
		{
			return value switch
			{
				global::System.DateTimeKind.Unspecified => nameof(global::System.DateTimeKind.Unspecified),
				global::System.DateTimeKind.Utc => nameof(global::System.DateTimeKind.Utc),
				global::System.DateTimeKind.Local => nameof(global::System.DateTimeKind.Local),
				_ => null,
			};
		}

		public static string? GetName(global::System.UriKind value)
		{
			return value switch
			{
				global::System.UriKind.RelativeOrAbsolute => nameof(global::System.UriKind.RelativeOrAbsolute),
				global::System.UriKind.Absolute => nameof(global::System.UriKind.Absolute),
				global::System.UriKind.Relative => nameof(global::System.UriKind.Relative),
				_ => null,
			};
		}

		public static string? GetName(global::System.UriFormat value)
		{
			return value switch
			{
				global::System.UriFormat.UriEscaped => nameof(global::System.UriFormat.UriEscaped),
				global::System.UriFormat.Unescaped => nameof(global::System.UriFormat.Unescaped),
				global::System.UriFormat.SafeUnescaped => nameof(global::System.UriFormat.SafeUnescaped),
				_ => null,
			};
		}

		public static string? GetName(global::System.UriPartial value)
		{
			return value switch
			{
				global::System.UriPartial.Scheme => nameof(global::System.UriPartial.Scheme),
				global::System.UriPartial.Authority => nameof(global::System.UriPartial.Authority),
				global::System.UriPartial.Path => nameof(global::System.UriPartial.Path),
				global::System.UriPartial.Query => nameof(global::System.UriPartial.Query),
				_ => null,
			};
		}

		public static string? GetName(global::System.UriHostNameType value)
		{
			return value switch
			{
				global::System.UriHostNameType.Unknown => nameof(global::System.UriHostNameType.Unknown),
				global::System.UriHostNameType.Basic => nameof(global::System.UriHostNameType.Basic),
				global::System.UriHostNameType.Dns => nameof(global::System.UriHostNameType.Dns),
				global::System.UriHostNameType.IPv4 => nameof(global::System.UriHostNameType.IPv4),
				global::System.UriHostNameType.IPv6 => nameof(global::System.UriHostNameType.IPv6),
				_ => null,
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

		_ = EnumInfo.GetName(System.IO.FileAccess.Read);
		_ = EnumInfo.GetName(System.IO.FileAccess.Write);
		_ = EnumInfo.GetName(System.IO.FileAccess.ReadWrite);
	}
}
";

		string generated = CreateGenerated(@"
		public static string? GetName(global::System.StringSplitOptions value)
		{
			return value switch
			{
				global::System.StringSplitOptions.None => nameof(global::System.StringSplitOptions.None),
				global::System.StringSplitOptions.RemoveEmptyEntries => nameof(global::System.StringSplitOptions.RemoveEmptyEntries),
				global::System.StringSplitOptions.TrimEntries => nameof(global::System.StringSplitOptions.TrimEntries),
				_ => null,
			};
		}

		public static string? GetName(global::System.ConsoleModifiers value)
		{
			return value switch
			{
				global::System.ConsoleModifiers.Alt => nameof(global::System.ConsoleModifiers.Alt),
				global::System.ConsoleModifiers.Shift => nameof(global::System.ConsoleModifiers.Shift),
				global::System.ConsoleModifiers.Control => nameof(global::System.ConsoleModifiers.Control),
				_ => null,
			};
		}

		public static string? GetName(global::System.IO.FileAccess value)
		{
			return value switch
			{
				global::System.IO.FileAccess.Read => nameof(global::System.IO.FileAccess.Read),
				global::System.IO.FileAccess.Write => nameof(global::System.IO.FileAccess.Write),
				global::System.IO.FileAccess.ReadWrite => nameof(global::System.IO.FileAccess.ReadWrite),
				_ => null,
			};
		}");

		await VerifyAsync(test, generated);
	}

	[Theory]
	[InlineData(GeneratorConfiguration.Unset, GeneratorConfiguration.Unset, LanguageVersion.CSharp8)]
	[InlineData(GeneratorConfiguration.Unset, GeneratorConfiguration.Unset, LanguageVersion.CSharp7_3)]
	[InlineData(GeneratorConfiguration.True, GeneratorConfiguration.Unset, LanguageVersion.CSharp8)]
	[InlineData(GeneratorConfiguration.True, GeneratorConfiguration.Unset, LanguageVersion.CSharp7_3)]
	[InlineData(GeneratorConfiguration.Unset, GeneratorConfiguration.Enable, LanguageVersion.CSharp8)]
	[InlineData(GeneratorConfiguration.Unset, GeneratorConfiguration.Enable, LanguageVersion.CSharp7_3)]
	public async Task Execute_Throw(string? throwAnalyzerConfig, string? throwMSBuildProperty, LanguageVersion languageVersion)
	{
		GeneratorConfiguration configuration = new(throwAnalyzerConfig, throwMSBuildProperty);

		string test =
@"using System;
using System.Reflection;
using F0.Generated;

public sealed class Class
{
	public void Method()
	{
		_ = EnumInfo.GetName(MemberTypes.Method);
	}
}
";

		StringBuilder code = new();

		code.AppendLine();
		_ = languageVersion <= LanguageVersion.CSharp7_3 || configuration.UseThrow()
			? code.AppendLine($"\t\tpublic static string GetName(global::System.Reflection.MemberTypes value)")
			: code.AppendLine($"\t\tpublic static string? GetName(global::System.Reflection.MemberTypes value)");
		code.AppendLine($"\t\t{{");
		_ = languageVersion >= LanguageVersion.CSharp8
			? code.AppendLine($"\t\t\treturn value switch")
			: code.AppendLine($"\t\t\tswitch (value)");
		code.AppendLine($"\t\t\t{{");
		if (languageVersion >= LanguageVersion.CSharp8)
		{
			code.AppendLine(@"				global::System.Reflection.MemberTypes.Constructor => nameof(global::System.Reflection.MemberTypes.Constructor),
				global::System.Reflection.MemberTypes.Event => nameof(global::System.Reflection.MemberTypes.Event),
				global::System.Reflection.MemberTypes.Field => nameof(global::System.Reflection.MemberTypes.Field),
				global::System.Reflection.MemberTypes.Method => nameof(global::System.Reflection.MemberTypes.Method),
				global::System.Reflection.MemberTypes.Property => nameof(global::System.Reflection.MemberTypes.Property),
				global::System.Reflection.MemberTypes.TypeInfo => nameof(global::System.Reflection.MemberTypes.TypeInfo),
				global::System.Reflection.MemberTypes.Custom => nameof(global::System.Reflection.MemberTypes.Custom),
				global::System.Reflection.MemberTypes.NestedType => nameof(global::System.Reflection.MemberTypes.NestedType),
				global::System.Reflection.MemberTypes.All => nameof(global::System.Reflection.MemberTypes.All),");
			_ = configuration.UseThrow()
				? code.AppendLine($"\t\t\t\t_ => throw new global::System.ComponentModel.InvalidEnumArgumentException(nameof(value), (int)value, typeof(global::System.Reflection.MemberTypes)),")
				: code.AppendLine($"\t\t\t\t_ => null,");
		}
		else
		{
			code.AppendLine(@"				case global::System.Reflection.MemberTypes.Constructor:
					return nameof(global::System.Reflection.MemberTypes.Constructor);
				case global::System.Reflection.MemberTypes.Event:
					return nameof(global::System.Reflection.MemberTypes.Event);
				case global::System.Reflection.MemberTypes.Field:
					return nameof(global::System.Reflection.MemberTypes.Field);
				case global::System.Reflection.MemberTypes.Method:
					return nameof(global::System.Reflection.MemberTypes.Method);
				case global::System.Reflection.MemberTypes.Property:
					return nameof(global::System.Reflection.MemberTypes.Property);
				case global::System.Reflection.MemberTypes.TypeInfo:
					return nameof(global::System.Reflection.MemberTypes.TypeInfo);
				case global::System.Reflection.MemberTypes.Custom:
					return nameof(global::System.Reflection.MemberTypes.Custom);
				case global::System.Reflection.MemberTypes.NestedType:
					return nameof(global::System.Reflection.MemberTypes.NestedType);
				case global::System.Reflection.MemberTypes.All:
					return nameof(global::System.Reflection.MemberTypes.All);
				default:");
			_ = configuration.UseThrow()
				? code.AppendLine($"\t\t\t\t\tthrow new global::System.ComponentModel.InvalidEnumArgumentException(nameof(value), (int)value, typeof(global::System.Reflection.MemberTypes));")
				: code.AppendLine($"\t\t\t\t\treturn null;");
		}
		_ = languageVersion >= LanguageVersion.CSharp8
			? code.AppendLine($"\t\t\t}};")
			: code.AppendLine($"\t\t\t}}");
		code.Append($"\t\t}}");

		string generated = CreateGenerated(code.ToString(), languageVersion, configuration);

		await VerifyAsync(test, generated, languageVersion, configuration);
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
					return null;
			}
		}", LanguageVersion.CSharp7_3);

		await VerifyAsync(test, generated, LanguageVersion.CSharp7_3);
	}

	[Theory]
	[InlineData(LanguageVersion.CSharp7_2)]
	[InlineData(LanguageVersion.CSharp6)]
	public async Task Execute_LanguageVersion_Supported(LanguageVersion langVersion)
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
					return nameof(global::System.Diagnostics.DebuggerBrowsableState.Never);
				case global::System.Diagnostics.DebuggerBrowsableState.Collapsed:
					return nameof(global::System.Diagnostics.DebuggerBrowsableState.Collapsed);
				case global::System.Diagnostics.DebuggerBrowsableState.RootHidden:
					return nameof(global::System.Diagnostics.DebuggerBrowsableState.RootHidden);
				default:
					return null;
			}
		}", langVersion);

		await VerifyAsync(test, generated, langVersion);
	}

	[Fact]
	public async Task Execute_LanguageVersion_NotSupported()
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

		string path = $@"F0.Generators\{typeof(EnumInfoGenerator).FullName}\EnumInfo.g.cs";
		DiagnosticResult[] diagnostics = new[]
		{
			CreateDiagnostic(LanguageVersion.CSharp5, LanguageVersion.CSharp6, LanguageFeatures.InterpolatedStrings).WithSpan(path, 9, 61, 9, 278),
			CreateDiagnostic(LanguageVersion.CSharp5, LanguageVersion.CSharp6, LanguageFeatures.NullPropagatingOperator).WithSpan(path, 9, 160, 9, 187),
			CreateDiagnostic(LanguageVersion.CSharp5, LanguageVersion.CSharp6, LanguageFeatures.NameofOperator).WithLocation(0),
			CreateDiagnostic(LanguageVersion.CSharp5, LanguageVersion.CSharp6, LanguageFeatures.NameofOperator).WithLocation(1),
			CreateDiagnostic(LanguageVersion.CSharp5, LanguageVersion.CSharp6, LanguageFeatures.NameofOperator).WithLocation(2),
			CreateDiagnostic(LanguageVersion.CSharp5, LanguageVersion.CSharp6, LanguageFeatures.NameofOperator).WithLocation(3),
			CreateDiagnostic(LanguageVersion.CSharp5, LanguageVersion.CSharp6, LanguageFeatures.NameofOperator).WithLocation(4),
		};

		string generated = CreateGenerated(@"
		public static string GetName(global::System.MidpointRounding value)
		{
			switch (value)
			{
				case global::System.MidpointRounding.ToEven:
					return {|#0:nameof(global::System.MidpointRounding.ToEven)|};
				case global::System.MidpointRounding.AwayFromZero:
					return {|#1:nameof(global::System.MidpointRounding.AwayFromZero)|};
				case global::System.MidpointRounding.ToZero:
					return {|#2:nameof(global::System.MidpointRounding.ToZero)|};
				case global::System.MidpointRounding.ToNegativeInfinity:
					return {|#3:nameof(global::System.MidpointRounding.ToNegativeInfinity)|};
				case global::System.MidpointRounding.ToPositiveInfinity:
					return {|#4:nameof(global::System.MidpointRounding.ToPositiveInfinity)|};
				default:
					return null;
			}
		}", LanguageVersion.CSharp5);

		await VerifyAsync(test, diagnostics, generated, LanguageVersion.CSharp5);
	}

	[Theory]
	[InlineData(LanguageVersion.Latest)]
	[InlineData(LanguageVersion.CSharp7_3)]
	[InlineData(LanguageVersion.CSharp6)]
	public async Task Execute_CheckForOverflowUnderflow(LanguageVersion version)
	{
		string test =
@"using System;
using F0.Generated;

public sealed class Class
{
	public void Method()
	{
		EnumInfo.GetName(ByteEnum.Constant);
		EnumInfo.GetName(SByteEnum.Constant);
		EnumInfo.GetName(Int16Enum.Constant);
		EnumInfo.GetName(UInt16Enum.Constant);
		EnumInfo.GetName(Int32Enum.Constant);
		EnumInfo.GetName(UInt32Enum.Constant);
		EnumInfo.GetName(Int64Enum.Constant);
		EnumInfo.GetName(UInt64Enum.Constant);
	}
}

internal enum ByteEnum : byte { Constant = 1 }
internal enum SByteEnum : sbyte { Constant = 1 }
internal enum Int16Enum : short { Constant = 1 }
internal enum UInt16Enum : ushort { Constant = 1 }
internal enum Int32Enum : int { Constant = 1 }
internal enum UInt32Enum : uint { Constant = 1 }
internal enum Int64Enum : long { Constant = 1 }
internal enum UInt64Enum : ulong { Constant = 1 }
";

		StringBuilder code = new();
		Type[] underlyingTypes = new[] { typeof(byte), typeof(sbyte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong) };

		for (int i = 0; i < underlyingTypes.Length; i++)
		{
			Type underlyingType = underlyingTypes[i];
			string invalidValue = i >= 5 ? "unchecked((int)value)" : "(int)value";

			code.AppendLine();

			if (version >= LanguageVersion.CSharp8)
			{
				code.AppendLine($"\t\tpublic static string GetName(global::{underlyingType.Name}Enum value)");
				code.AppendLine($"\t\t{{");
				code.AppendLine($"\t\t\treturn value switch");
				code.AppendLine($"\t\t\t{{");
				code.AppendLine($"\t\t\t\tglobal::{underlyingType.Name}Enum.Constant => nameof(global::{underlyingType.Name}Enum.Constant),");
				code.AppendLine($"\t\t\t\t_ => throw new global::System.ComponentModel.InvalidEnumArgumentException(nameof(value), {invalidValue}, typeof(global::{underlyingType.Name}Enum)),");
				code.AppendLine($"\t\t\t}};");
			}
			else
			{
				code.AppendLine($"\t\tpublic static string GetName(global::{underlyingType.Name}Enum value)");
				code.AppendLine($"\t\t{{");
				code.AppendLine($"\t\t\tswitch (value)");
				code.AppendLine($"\t\t\t{{");
				code.AppendLine($"\t\t\t\tcase global::{underlyingType.Name}Enum.Constant:");
				code.AppendLine($"\t\t\t\t\treturn nameof(global::{underlyingType.Name}Enum.Constant);");
				code.AppendLine($"\t\t\t\tdefault:");
				code.AppendLine($"\t\t\t\t\tthrow new global::System.ComponentModel.InvalidEnumArgumentException(nameof(value), {invalidValue}, typeof(global::{underlyingType.Name}Enum));");
				code.AppendLine($"\t\t\t}}");
			}

			_ = i == 7
				? code.Append($"\t\t}}")
				: code.AppendLine($"\t\t}}");
		}

		string generated = CreateGenerated(code.ToString(), version, GeneratorConfiguration.Throw);

		await VerifyAsync(test, generated, version, GeneratorConfiguration.Throw, OverflowCheck.Checked);
	}

	[Theory]
	[MemberData(nameof(ConfigurationData))]
	public async Task Execute_Configuration(string? throwAnalyzerConfig, string? throwMSBuildProperty)
	{
		GeneratorConfiguration configuration = new(throwAnalyzerConfig, throwMSBuildProperty);

		string test =
@"#nullable enable
using System;
using System.IO;
using System.Net.Sockets;
using F0.Generated;

public sealed class Class
{
	public void Method(MyEnum value)
	{
		_ = EnumInfo.GetName(value);
	}
}

[Flags]
public enum MyEnum
{
	None = 0x00,
	First = 0x01,
	Second = 0x02,
	All = First | Second,
}
";

		List<DiagnosticResult> diagnostics = new(1);
		if (configuration.IsAmbiguous())
		{
			DiagnosticResult diagnostic = CreateAmbiguousConfigurationDiagnostic(configuration);
			diagnostics.Add(diagnostic);
		}

		string generated = CreateGenerated($@"
		{(configuration.UseNullable()
			? "public static string? GetName(global::MyEnum value)"
			: "public static string GetName(global::MyEnum value)")}
		{{
			return value switch
			{{
				global::MyEnum.None => nameof(global::MyEnum.None),
				global::MyEnum.First => nameof(global::MyEnum.First),
				global::MyEnum.Second => nameof(global::MyEnum.Second),
				global::MyEnum.All => nameof(global::MyEnum.All),
				{(configuration.UseNullable()
					? "_ => null,"
					: "_ => throw new global::System.ComponentModel.InvalidEnumArgumentException(nameof(value), (int)value, typeof(global::MyEnum)),")}
			}};
		}}", LanguageVersion.Latest, configuration);

		await VerifyAsync(test, diagnostics.ToArray(), generated, LanguageVersion.Latest, configuration, OverflowCheck.Unset);
	}

	private static TheoryData<string?, string?> ConfigurationData()
	{
		TheoryData<string?, string?> data = new();

		string?[] properties = { GeneratorConfiguration.Unset, GeneratorConfiguration.Empty, GeneratorConfiguration.Enable, GeneratorConfiguration.Disable, GeneratorConfiguration.Invalid };

		foreach (string? analyzerConfig in new[] { GeneratorConfiguration.Unset, GeneratorConfiguration.Empty, GeneratorConfiguration.True, GeneratorConfiguration.False, GeneratorConfiguration.Invalid })
		{
			foreach (string? buildProperty in properties)
			{
				data.Add(analyzerConfig, buildProperty);
			}
		}

		return data;
	}

	private static string CreateGenerated(string? code, LanguageVersion? languageVersion = null, GeneratorConfiguration? configuration = null)
	{
		string source = code is null ? String.Empty : Environment.NewLine + code;
		LanguageVersion version = languageVersion.GetValueOrDefault(LanguageVersion.Latest);
		GeneratorConfiguration config = configuration ?? GeneratorConfiguration.Default;

		string methodDeclaration = version switch
		{
			>= LanguageVersion.CSharp8 when config.UseNullable() => "public static string? GetName(global::System.Enum? value)",
			>= LanguageVersion.CSharp8 => "public static string GetName(global::System.Enum? value)",
			_ => "public static string GetName(global::System.Enum value)",
		};

		return $@"namespace F0.Generated
{{
	internal static class EnumInfo
	{{
		{methodDeclaration}
		{{
			throw new global::F0.Generated.SourceGenerationException($""Cannot use the unspecialized method, which serves as a placeholder for the generator. Enum-Type {{value?.GetType().ToString() ?? ""<null>""}} must be concrete to generate the allocation-free variant of {nameof(Enum)}.{nameof(Enum.ToString)}()."");
		}}{source}
	}}
}}
";
	}

	private static DiagnosticResult CreateDiagnostic(string diagnosticId, DiagnosticSeverity severity)
		=> CSharpSourceGeneratorVerifier<EnumInfoGenerator>.Diagnostic(diagnosticId, severity);

	private static DiagnosticResult CreateUnspecializedPlaceholderDiagnostic(int markupKey, string argument)
	{
		const string method = "EnumInfo.GetName(Enum)";

		return CSharpSourceGeneratorVerifier<EnumInfoGenerator>.Diagnostic(DiagnosticIds.F0GEN0301, DiagnosticSeverity.Error)
			.WithMessageFormat($"Do not use the unspecialized placeholder method of '{method}' with non-enum argument '{{0}}'")
			.WithArguments(argument)
			.WithLocation(markupKey);
	}

	private static DiagnosticResult CreateDiagnostic(LanguageVersion current, LanguageVersion required, string feature)
		=> CSharpSourceGeneratorVerifier<EnumInfoGenerator>.Diagnostic(current, required, feature);

	private static DiagnosticResult CreateAmbiguousConfigurationDiagnostic(GeneratorConfiguration configuration)
	{
		const string message = "Ambiguous configuration of 'EnumInfoGenerator': "
			+ "Global AnalyzerConfig: 'f0gen_enum_throw = {0}'"
			+ " -and- "
			+ "MSBuild Property: '<F0Gen_EnumInfo_ThrowIfConstantNotFound>{1}</F0Gen_EnumInfo_ThrowIfConstantNotFound>'";

		Debug.Assert(configuration.IsAmbiguous(), "Configuration should be ambiguous.");

		return CSharpSourceGeneratorVerifier<EnumInfoGenerator>.Diagnostic(DiagnosticIds.F0GEN0302, DiagnosticSeverity.Warning)
			.WithMessageFormat(message)
			.WithArguments(configuration.ThrowAnalyzerConfig, configuration.ThrowMSBuildProperty);
	}

	private static Task VerifyAsync(string test, string generated, LanguageVersion? languageVersion = null, GeneratorConfiguration? configuration = null, OverflowCheck checkOverflow = default)
		=> VerifyAsync(test, Array.Empty<DiagnosticResult>(), generated, languageVersion, configuration, checkOverflow);

	private static Task VerifyAsync(string test, DiagnosticResult[] diagnostics, string generated, LanguageVersion? languageVersion = null)
		=> VerifyAsync(test, diagnostics, generated, languageVersion, null, default);

	private static Task VerifyAsync(string test, DiagnosticResult[] diagnostics, string generated, LanguageVersion? languageVersion, GeneratorConfiguration? configuration, OverflowCheck checkOverflow)
	{
		string filename = $@"F0.Generators\{typeof(EnumInfoGenerator).FullName}\EnumInfo.g.cs";
		string content = String.Concat(Sources.GetFileHeader(languageVersion), generated);

		test += Sources.SourceGenerationException_String;

		CSharpSourceGeneratorVerifier<EnumInfoGenerator>.Test verifier = CSharpSourceGeneratorVerifier<EnumInfoGenerator>.Create(test, diagnostics, (filename, content), languageVersion, ReferenceAssemblies.Net.Net50);

		if (configuration is not null)
		{
			if (configuration.IsAnalyzerConfigSet())
			{
				string config = $"is_global = true{Environment.NewLine}f0gen_enum_throw = {configuration.ThrowAnalyzerConfig}";
				verifier.TestState.AnalyzerConfigFiles.Add(("/.globalconfig", config));
			}

			if (configuration.IsMSBuildPropertySet())
			{
				string config = $"is_global = true{Environment.NewLine}build_property.F0Gen_EnumInfo_ThrowIfConstantNotFound = {configuration.ThrowMSBuildProperty}";
				verifier.TestState.AnalyzerConfigFiles.Add(("/.globalconfig", config));
			}
		}

		verifier.CheckOverflow = checkOverflow switch
		{
			OverflowCheck.Unset => null,
			OverflowCheck.Unchecked => false,
			OverflowCheck.Checked => true,
			_ => throw new InvalidEnumArgumentException(nameof(checkOverflow), (int)checkOverflow, typeof(OverflowCheck)),
		};

		return verifier.RunAsync(CancellationToken.None);
	}

	private sealed class GeneratorConfiguration
	{
		public const string? Unset = null;
		public const string Empty = "";
		public const string True = "true";
		public const string False = "false";
		public const string Enable = "enable";
		public const string Disable = "disable";
		public const string Invalid = "invalid";

		public static GeneratorConfiguration Default { get; } = new GeneratorConfiguration(Unset, Unset);
		public static GeneratorConfiguration Throw { get; } = new GeneratorConfiguration(True, Enable);

		public GeneratorConfiguration(string? throwAnalyzerConfig, string? throwMSBuildProperty)
		{
			ThrowAnalyzerConfig = throwAnalyzerConfig;
			ThrowMSBuildProperty = throwMSBuildProperty;

			ThrowIfInvalid();
		}

		public string? ThrowAnalyzerConfig { get; }
		public string? ThrowMSBuildProperty { get; }

		public bool UseNullable()
		{
			return (ThrowAnalyzerConfig == False && ThrowMSBuildProperty is False or Disable)
				|| (ThrowAnalyzerConfig is Unset or Empty or Invalid && ThrowMSBuildProperty is Unset or Empty or Invalid)
				|| !UseThrow()
				|| IsAmbiguous();
		}

		public bool UseThrow()
		{
			return (ThrowAnalyzerConfig == True && ThrowMSBuildProperty is True or Enable or Unset or Empty or Invalid)
				|| (ThrowAnalyzerConfig is True or Unset or Empty or Invalid && ThrowMSBuildProperty is True or Enable);
		}

		[MemberNotNullWhen(true, nameof(ThrowAnalyzerConfig), nameof(ThrowMSBuildProperty))]
		public bool IsAmbiguous()
		{
			return (ThrowAnalyzerConfig == True && ThrowMSBuildProperty is False or Disable)
				|| (ThrowAnalyzerConfig == False && ThrowMSBuildProperty is True or Enable);
		}

		public bool IsAnalyzerConfigSet()
			=> ThrowAnalyzerConfig != Unset;

		public bool IsMSBuildPropertySet()
			=> ThrowMSBuildProperty != Unset;

		private void ThrowIfInvalid()
		{
			if (UseNullable() && UseThrow())
			{
				throw new InvalidOperationException();
			}
		}
	}

	private enum OverflowCheck
	{
		Unset,
		Unchecked,
		Checked,
	}
}
