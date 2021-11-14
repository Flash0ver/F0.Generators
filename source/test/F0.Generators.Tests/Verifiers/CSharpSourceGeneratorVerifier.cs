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
			=> new DiagnosticResult();

		public static DiagnosticResult Diagnostic(string diagnosticId, DiagnosticSeverity severity)
			=> new DiagnosticResult(diagnosticId, severity);

		public static DiagnosticResult Diagnostic(DiagnosticDescriptor descriptor)
			=> new DiagnosticResult(descriptor);

		public static Task VerifySourceGeneratorAsync(string source, params DiagnosticResult[] expected)
			=> VerifySourceGeneratorAsync(source, expected, default, default);

		public static Task VerifySourceGeneratorAsync(string source, DiagnosticResult[] expected, LanguageVersion? languageVersion = null, ReferenceAssemblies? referenceAssemblies = null)
		{
			Test test = Create(source, expected, languageVersion, referenceAssemblies);

			return test.RunAsync(CancellationToken.None);
		}

		public static Test Create(string source, params DiagnosticResult[] expected)
			=> Create(source, expected, default, default);

		public static Test Create(string source, DiagnosticResult[] expected, LanguageVersion? languageVersion = null, ReferenceAssemblies? referenceAssemblies = null)
		{
			Test test = new()
			{
				TestCode = source,
			};

			if (expected.Length != 0)
			{
				test.ExpectedDiagnostics.AddRange(expected);
			}

			if (languageVersion.HasValue)
			{
				test.LanguageVersion = languageVersion;
			}

			if (referenceAssemblies is not null)
			{
				test.TestState.ReferenceAssemblies = referenceAssemblies;
			}

			return test;
		}

		public static Task VerifySourceGeneratorAsync(string source, (string filename, string content) generatedSource, LanguageVersion? languageVersion = null, ReferenceAssemblies? referenceAssemblies = null)
			=> VerifySourceGeneratorAsync(source, DiagnosticResult.EmptyDiagnosticResults, new[] { generatedSource }, languageVersion, referenceAssemblies);

		public static Task VerifySourceGeneratorAsync(string source, (string filename, string content)[] generatedSources, LanguageVersion? languageVersion = null, ReferenceAssemblies? referenceAssemblies = null)
			=> VerifySourceGeneratorAsync(source, DiagnosticResult.EmptyDiagnosticResults, generatedSources, languageVersion, referenceAssemblies);

		public static Task VerifySourceGeneratorAsync(string source, DiagnosticResult expected, (string filename, string content) generatedSource, LanguageVersion? languageVersion = null, ReferenceAssemblies? referenceAssemblies = null)
			=> VerifySourceGeneratorAsync(source, new[] { expected }, new[] { generatedSource }, languageVersion, referenceAssemblies);

		public static Task VerifySourceGeneratorAsync(string source, DiagnosticResult[] expected, (string filename, string content) generatedSource, LanguageVersion? languageVersion = null, ReferenceAssemblies? referenceAssemblies = null)
			=> VerifySourceGeneratorAsync(source, expected, new[] { generatedSource }, languageVersion, referenceAssemblies);

		public static Task VerifySourceGeneratorAsync(string source, DiagnosticResult expected, (string filename, string content)[] generatedSources, LanguageVersion? languageVersion = null, ReferenceAssemblies? referenceAssemblies = null)
			=> VerifySourceGeneratorAsync(source, new[] { expected }, generatedSources, languageVersion, referenceAssemblies);

		public static Task VerifySourceGeneratorAsync(string source, DiagnosticResult[] expected, (string filename, string content)[] generatedSources, LanguageVersion? languageVersion = null, ReferenceAssemblies? referenceAssemblies = null)
		{
			Test test = Create(source, expected, generatedSources, languageVersion, referenceAssemblies);

			return test.RunAsync(CancellationToken.None);
		}

		public static Test Create(string source, (string filename, string content) generatedSource, LanguageVersion? languageVersion = null, ReferenceAssemblies? referenceAssemblies = null)
			=> Create(source, DiagnosticResult.EmptyDiagnosticResults, new[] { generatedSource }, languageVersion, referenceAssemblies);

		public static Test Create(string source, (string filename, string content)[] generatedSources, LanguageVersion? languageVersion = null, ReferenceAssemblies? referenceAssemblies = null)
			=> Create(source, DiagnosticResult.EmptyDiagnosticResults, generatedSources, languageVersion, referenceAssemblies);

		public static Test Create(string source, DiagnosticResult expected, (string filename, string content) generatedSource, LanguageVersion? languageVersion = null, ReferenceAssemblies? referenceAssemblies = null)
			=> Create(source, new[] { expected }, new[] { generatedSource }, languageVersion, referenceAssemblies);

		public static Test Create(string source, DiagnosticResult[] expected, (string filename, string content) generatedSource, LanguageVersion? languageVersion = null, ReferenceAssemblies? referenceAssemblies = null)
			=> Create(source, expected, new[] { generatedSource }, languageVersion, referenceAssemblies);

		public static Test Create(string source, DiagnosticResult expected, (string filename, string content)[] generatedSources, LanguageVersion? languageVersion = null, ReferenceAssemblies? referenceAssemblies = null)
			=> Create(source, new[] { expected }, generatedSources, languageVersion, referenceAssemblies);

		public static Test Create(string source, DiagnosticResult[] expected, (string filename, string content)[] generatedSources, LanguageVersion? languageVersion = null, ReferenceAssemblies? referenceAssemblies = null)
		{
			Test test = new()
			{
				TestCode = source,
			};

			UTF8Encoding encoding = new(false, true);
			foreach ((string filename, string content) generatedSource in generatedSources)
			{
				SourceText code = SourceText.From(generatedSource.content, encoding);
				test.TestState.GeneratedSources.Add((generatedSource.filename, code));
			}

			if (expected.Length != 0)
			{
				test.ExpectedDiagnostics.AddRange(expected);
			}

			if (languageVersion.HasValue)
			{
				test.LanguageVersion = languageVersion;
			}

			if (referenceAssemblies is not null)
			{
				test.TestState.ReferenceAssemblies = referenceAssemblies;
			}

			return test;
		}
	}
}
