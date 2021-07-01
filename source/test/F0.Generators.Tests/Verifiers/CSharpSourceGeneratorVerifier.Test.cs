using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace F0.Tests.Verifiers
{
	internal static partial class CSharpSourceGeneratorVerifier<TSourceGenerator>
		where TSourceGenerator : ISourceGenerator, new()
	{
		public class Test : CSharpSourceGeneratorTest<TSourceGenerator, XUnitVerifier>
		{
			public Test()
			{
				SolutionTransforms.Add((solution, projectId) =>
				{
					Project? project = solution.GetProject(projectId);
					Debug.Assert(project is not null);

					CompilationOptions? compilationOptions = project.CompilationOptions;
					Debug.Assert(compilationOptions is not null);

					ImmutableDictionary<string, ReportDiagnostic> specificDiagnosticOptions = compilationOptions.SpecificDiagnosticOptions.SetItems(CSharpVerifierHelper.NullableWarnings);
					compilationOptions = compilationOptions.WithSpecificDiagnosticOptions(specificDiagnosticOptions);
					solution = solution.WithProjectCompilationOptions(projectId, compilationOptions);

					return solution;
				});
			}

			internal LanguageVersion? LanguageVersion { get; set; }

			protected override ParseOptions CreateParseOptions()
			{
				CSharpParseOptions options = (CSharpParseOptions)base.CreateParseOptions();

				return LanguageVersion.HasValue
					? options.WithLanguageVersion(LanguageVersion.Value)
					: options;
			}
		}
	}
}
