using System.Text;

namespace F0.Tests.Verifiers;

internal static partial class CSharpSourceGeneratorVerifier<TSourceGenerator>
	where TSourceGenerator : IIncrementalGenerator, new()
{
	private static readonly UTF8Encoding encoding = new(false, true);

	public static DiagnosticResult Diagnostic()
		=> new();

	public static DiagnosticResult Diagnostic(string diagnosticId, DiagnosticSeverity severity)
		=> new(diagnosticId, severity);

	public static DiagnosticResult Diagnostic(DiagnosticDescriptor descriptor)
		=> new(descriptor);

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

		foreach ((string filename, string content) in generatedSources)
		{
			var code = SourceText.From(content, encoding);
			test.TestState.GeneratedSources.Add((filename, code));
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
