using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Text;

namespace F0.Tests.Verifiers
{
	internal static partial class CSharpSourceGeneratorVerifier<TSourceGenerator>
		where TSourceGenerator : ISourceGenerator, new()
	{
		public static DiagnosticResult Diagnostic()
		{
			return new DiagnosticResult();
		}

		public static DiagnosticResult Diagnostic(string diagnosticId, DiagnosticSeverity severity)
		{
			return new DiagnosticResult(diagnosticId, severity);
		}

		public static DiagnosticResult Diagnostic(DiagnosticDescriptor descriptor)
		{
			return new DiagnosticResult(descriptor);
		}

		public static Task VerifySourceGeneratorAsync(string source, params DiagnosticResult[] expected)
		{
			return VerifySourceGeneratorAsync(source, expected, default, default);
		}

		public static Task VerifySourceGeneratorAsync(string source, DiagnosticResult[] expected, LanguageVersion? languageVersion = null, ReferenceAssemblies? referenceAssemblies = null)
		{
			Test test = new()
			{
				TestCode = source,
			};

			test.ExpectedDiagnostics.AddRange(expected);

			if (languageVersion.HasValue)
			{
				test.LanguageVersion = languageVersion;
			}

			if (referenceAssemblies is not null)
			{
				test.TestState.ReferenceAssemblies = referenceAssemblies;
			}

			return test.RunAsync(CancellationToken.None);
		}

		public static Task VerifySourceGeneratorAsync(string source, (string filename, string content) generatedSource, LanguageVersion? languageVersion = null, ReferenceAssemblies? referenceAssemblies = null)
		{
			return VerifySourceGeneratorAsync(source, DiagnosticResult.EmptyDiagnosticResults, generatedSource, languageVersion, referenceAssemblies);
		}

		public static Task VerifySourceGeneratorAsync(string source, DiagnosticResult expected, (string filename, string content) generatedSource, LanguageVersion? languageVersion = null, ReferenceAssemblies? referenceAssemblies = null)
		{
			return VerifySourceGeneratorAsync(source, new[] { expected }, generatedSource, languageVersion, referenceAssemblies);
		}

		public static Task VerifySourceGeneratorAsync(string source, DiagnosticResult[] expected, (string filename, string content) generatedSource, LanguageVersion? languageVersion = null, ReferenceAssemblies? referenceAssemblies = null)
		{
			Test test = new()
			{
				TestCode = source,
			};

			SourceText code = SourceText.From(generatedSource.content, new UTF8Encoding(false, true));
			test.TestState.GeneratedSources.Add((generatedSource.filename, code));

			test.ExpectedDiagnostics.AddRange(expected);

			if (languageVersion.HasValue)
			{
				test.LanguageVersion = languageVersion;
			}

			if (referenceAssemblies is not null)
			{
				test.TestState.ReferenceAssemblies = referenceAssemblies;
			}

			return test.RunAsync(CancellationToken.None);
		}
	}
}
