using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading;
using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace F0.Benchmarks.Measurers
{
	internal sealed class CSharpSourceGeneratorMeasurer<TSourceGenerator>
		where TSourceGenerator : ISourceGenerator, new()
	{
		private readonly TSourceGenerator generator;
		private Compilation? input;
		private Compilation? output;
		private GeneratorDriver? driver;
		private ImmutableArray<Diagnostic> diagnostics;

		internal CSharpSourceGeneratorMeasurer()
		{
			generator = new TSourceGenerator();
		}

		internal void Initialize(string source)
		{
			input = CreateCompilation(source);

			driver = CSharpGeneratorDriver.Create(ImmutableArray.Create<ISourceGenerator>(generator));
		}

		internal void Invoke()
		{
			Debug.Assert(driver is not null, $"Call {nameof(Initialize)} before {nameof(Invoke)}");
			Debug.Assert(input is not null, $"Call {nameof(Initialize)} before {nameof(Invoke)}");

			driver = driver.RunGeneratorsAndUpdateCompilation(input, out output, out diagnostics, CancellationToken.None);
		}

		internal void Inspect(string expectedSource)
		{
			Inspect(expectedSource, ImmutableArray<Diagnostic>.Empty);
		}

		internal void Inspect(string expectedSource, ImmutableArray<Diagnostic> expectedDiagnostics)
		{
			Debug.Assert(output is not null, $"Call {nameof(Invoke)} before {nameof(Inspect)}");
			Debug.Assert(driver is not null, $"Call {nameof(Invoke)} before {nameof(Inspect)}");
			Debug.Assert(input is not null, $"Call {nameof(Invoke)} before {nameof(Inspect)}");

			ImmutableArray<Diagnostic> outputDiagnostics = output.GetDiagnostics(CancellationToken.None);

			if (!outputDiagnostics.IsEmpty)
			{
				string message = $"{outputDiagnostics.Length} {nameof(Diagnostic)}{(outputDiagnostics.Length == 1 ? String.Empty : "s")} reported:"
					+ Environment.NewLine + String.Join(Environment.NewLine, outputDiagnostics);
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

				if (!diagnostic.Equals(expectedDiagnostic))
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

			if (!ReferenceEquals(generatorResult.Generator, generator))
			{
				string message = $"Unexpected {nameof(ISourceGenerator)}:"
					+ Environment.NewLine + $"Expected: '{generator}'"
					+ Environment.NewLine + $"Actual: '{generatorResult.Generator}'.";
				throw new InvalidOperationException(message);
			}

			if (generatorResult.GeneratedSources.Length != 1)
			{
				string message = $"Expected one generated source, but found {generatorResult.GeneratedSources.Length}.";
				throw new InvalidOperationException(message);
			}
			if (generatorResult.Exception is not null)
			{
				string message = $"The {nameof(ISourceGenerator)} '{generatorResult.Generator}' threw an unhandled exception:"
					+ Environment.NewLine + generatorResult.Exception;
				throw new InvalidOperationException(message);
			}

			ImmutableArray<SyntaxTree> inputTrees = input.SyntaxTrees.ToImmutableArray();
			ImmutableArray<SyntaxTree> outputTrees = output.SyntaxTrees.ToImmutableArray();

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
				string message = "Expected and actual source text differ: " + Environment.NewLine + diff;
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

		private static string Diff(string original, string modified)
		{
			StringBuilder diffText = new();

			Differ differ = new();
			InlineDiffBuilder diffBuilder = new(differ);
			DiffPaneModel diffModel = diffBuilder.BuildDiffModel(original, modified, false);

			foreach (DiffPiece diffPiece in diffModel.Lines)
			{
				switch (diffPiece.Type)
				{
					case ChangeType.Inserted:
						_ = diffText.Append('+');
						break;
					case ChangeType.Deleted:
						_ = diffText.Append('-');
						break;
					default:
						_ = diffText.Append(' ');
						break;
				}

				_ = diffText.AppendLine(diffPiece.Text);
			}

			return diffText.ToString();
		}
	}
}
