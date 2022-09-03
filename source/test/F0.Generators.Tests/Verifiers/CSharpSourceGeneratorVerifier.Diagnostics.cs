using System.ComponentModel;

namespace F0.Tests.Verifiers;

internal static partial class CSharpSourceGeneratorVerifier<TSourceGenerator>
{
	public static DiagnosticResult Diagnostic(LanguageVersion current, LanguageVersion required, string feature)
	{
		string id = Id(current);
		string messageFormat = MessageFormat(current);
		DiagnosticSeverity defaultSeverity = DiagnosticSeverity.Error;

		DiagnosticDescriptor descriptor = new(id, String.Empty, messageFormat, String.Empty, defaultSeverity, default);

		return new DiagnosticResult(descriptor)
			.WithArguments(feature, required.ToDisplayString());

		static string Id(LanguageVersion langVersion)
		{
			return langVersion switch
			{
				LanguageVersion.CSharp1 => "CS8022",
				LanguageVersion.CSharp2 => "CS8023",
				LanguageVersion.CSharp5 => "CS8026",
				_ => throw new InvalidEnumArgumentException(nameof(langVersion), (int)langVersion, typeof(LanguageVersion)),
			};
		}

		static string MessageFormat(LanguageVersion langVersion)
		{
			return langVersion switch
			{
				LanguageVersion.CSharp1 => "Feature '{0}' is not available in C# 1. Please use language version {1} or greater.",
				LanguageVersion.CSharp2 => "Feature '{0}' is not available in C# 2. Please use language version {1} or greater.",
				LanguageVersion.CSharp5 => "Feature '{0}' is not available in C# 5. Please use language version {1} or greater.",
				_ => throw new InvalidEnumArgumentException(nameof(langVersion), (int)langVersion, typeof(LanguageVersion)),
			};
		}
	}
}
