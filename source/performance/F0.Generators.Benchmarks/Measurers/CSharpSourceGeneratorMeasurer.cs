using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;
using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace F0.Benchmarks.Measurers;

internal sealed class CSharpSourceGeneratorMeasurer<TSourceGenerator>
	where TSourceGenerator : IIncrementalGenerator, new()
{
	private readonly TSourceGenerator generator;
	private IEnumerable<ISourceGenerator>? generators;
	private Compilation? input;
	private GeneratorDriver? driver;

	internal CSharpSourceGeneratorMeasurer()
		=> generator = new TSourceGenerator();

	internal void Initialize(string source)
	{
		input = CreateCompilation(source);

		generators = ImmutableArray.Create(generator.AsSourceGenerator());
		driver = CSharpGeneratorDriver.Create(generators);
	}

	internal void Invoke()
	{
		Debug.Assert(driver is not null, $"Call {nameof(Initialize)} before {nameof(Invoke)}");
		Debug.Assert(input is not null, $"Call {nameof(Initialize)} before {nameof(Invoke)}");

		_ = driver.RunGeneratorsAndUpdateCompilation(input, out _, out _, CancellationToken.None);
	}

	internal void Inspect(string expectedSource)
		=> Inspect(expectedSource, ImmutableArray<Diagnostic>.Empty);

	internal void Inspect(string expectedSource, Diagnostic[] expectedDiagnostics)
		=> Inspect(expectedSource, expectedDiagnostics.ToImmutableArray());

	internal void Inspect(string expectedSource, ImmutableArray<Diagnostic> expectedDiagnostics)
	{
		Debug.Assert(generators is not null, $"Call {nameof(Initialize)} before {nameof(Inspect)}");
		Debug.Assert(driver is not null, $"Call {nameof(Initialize)} before {nameof(Inspect)}");
		Debug.Assert(input is not null, $"Call {nameof(Initialize)} before {nameof(Inspect)}");

		driver = driver.RunGeneratorsAndUpdateCompilation(input, out Compilation output, out ImmutableArray<Diagnostic> diagnostics, CancellationToken.None);

		ImmutableArray<Diagnostic> outputDiagnostics = output.GetDiagnostics(CancellationToken.None);

		if (!outputDiagnostics.IsEmpty)
		{
			string message = $"""
				{outputDiagnostics.Length} {nameof(Diagnostic)}{(outputDiagnostics.Length == 1 ? String.Empty : "s")} reported:
				{String.Join(Environment.NewLine, outputDiagnostics)}
				""";
			throw new InvalidOperationException(message);
		}

		if (diagnostics.Length != expectedDiagnostics.Length)
		{
			string message = $"Mismatch between number of diagnostics reported, expected '{expectedDiagnostics.Length}' actual '{diagnostics.Length}'.";
			throw new InvalidOperationException(message);
		}

		for (int i = 0; i < diagnostics.Length; i++)
		{
			Diagnostic diagnostic = diagnostics[i];
			Diagnostic expectedDiagnostic = expectedDiagnostics[i];

			if (!Equal(expectedDiagnostic, diagnostic))
			{
				string message = $"Expected reported {nameof(Diagnostic)} #{i} to be '{diagnostic}', but actually was '{expectedDiagnostic}'.";
				throw new InvalidOperationException(message);
			}
		}

		GeneratorDriverRunResult runResult = driver.GetRunResult();

		if (runResult.GeneratedTrees.Length != 1)
		{
			string message = $"Expected one generated {nameof(SyntaxTree)}, but {runResult.GeneratedTrees.Length} were generated.";
			throw new InvalidOperationException(message);
		}

		GeneratorRunResult generatorResult = runResult.Results[0];

		if (!ReferenceEquals(generatorResult.Generator, generators.Single()))
		{
			string message = $"""
				Unexpected {nameof(ISourceGenerator)}:
				Expected: '{generators.Single()}'
				Actual: '{generatorResult.Generator}'
				""";
			throw new InvalidOperationException(message);
		}

		if (generatorResult.GeneratedSources.Length != 1)
		{
			string message = $"Expected one generated source, but found {generatorResult.GeneratedSources.Length}.";
			throw new InvalidOperationException(message);
		}
		if (generatorResult.Exception is not null)
		{
			string message = $"""
				The {nameof(ISourceGenerator)} '{generatorResult.Generator}' threw an unhandled exception:
				{generatorResult.Exception}
				""";
			throw new InvalidOperationException(message);
		}

		var inputTrees = input.SyntaxTrees.ToImmutableArray();
		var outputTrees = output.SyntaxTrees.ToImmutableArray();

		if (inputTrees.Length + 1 != outputTrees.Length)
		{
			string message = $"{generator.GetType()} should have generated one {nameof(SyntaxTree)}, but actually generated {outputTrees.Length - inputTrees.Length}.";
			throw new InvalidOperationException(message);
		}

		SyntaxTree generated = outputTrees[^1];

		string actualSource = generated.ToString();

		if (!actualSource.Equals(expectedSource, StringComparison.Ordinal))
		{
			string diff = Diff(expectedSource, actualSource);
			string message = $"""
				Expected and actual source text differ:
				{diff}
				""";
			throw new InvalidOperationException(message);
		}
	}

	private static Compilation CreateCompilation(string source)
	{
		return CSharpCompilation.Create("generated",
			new[] { CSharpSyntaxTree.ParseText(source) },
			new[] { MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location) },
			new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
	}

	private static bool Equal(Diagnostic expected, Diagnostic actual)
	{
		return expected.Id.Equals(actual.Id, StringComparison.Ordinal)
			&& expected.GetMessage(CultureInfo.InvariantCulture).Equals(actual.GetMessage(CultureInfo.InvariantCulture), StringComparison.Ordinal)
			&& expected.Severity == actual.Severity
			&& expected.DefaultSeverity == actual.DefaultSeverity
			&& expected.WarningLevel == actual.WarningLevel
			&& expected.IsSuppressed == actual.IsSuppressed;
	}

	private static string Diff(string original, string modified)
	{
		StringBuilder diffText = new();

		Differ differ = new();
		InlineDiffBuilder diffBuilder = new(differ);
		DiffPaneModel diffModel = diffBuilder.BuildDiffModel(original, modified, false);

		foreach (DiffPiece diffPiece in diffModel.Lines)
		{
			_ = diffPiece.Type switch
			{
				ChangeType.Inserted => diffText.Append('+'),
				ChangeType.Deleted => diffText.Append('-'),
				_ => diffText.Append(' '),
			};
			_ = diffText.AppendLine(diffPiece.Text);
		}

		return diffText.ToString();
	}
}
