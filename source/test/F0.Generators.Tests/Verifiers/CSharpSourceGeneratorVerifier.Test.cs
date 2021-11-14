using System.Collections.Immutable;
using System.Diagnostics;

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
					Debug.Assert(project is not null, $"{nameof(ProjectId)} '{projectId}' is not an id of a project that is part of the {nameof(Solution)} {solution}.");

					CompilationOptions? compilationOptions = project.CompilationOptions;
					Debug.Assert(compilationOptions is not null);

					ImmutableDictionary<string, ReportDiagnostic> specificDiagnosticOptions = compilationOptions.SpecificDiagnosticOptions.SetItems(CSharpVerifierHelper.NullableWarnings);
					compilationOptions = compilationOptions.WithSpecificDiagnosticOptions(specificDiagnosticOptions);
					solution = solution.WithProjectCompilationOptions(projectId, compilationOptions);

					return solution;
				});
			}

			internal bool? CheckOverflow { get; set; }
			internal LanguageVersion? LanguageVersion { get; set; }

			protected override CompilationOptions CreateCompilationOptions()
			{
				var options = (CSharpCompilationOptions)base.CreateCompilationOptions();

				return CheckOverflow.HasValue
					? options.WithOverflowChecks(CheckOverflow.Value)
					: options;
			}

			protected override ParseOptions CreateParseOptions()
			{
				var options = (CSharpParseOptions)base.CreateParseOptions();

				return LanguageVersion.HasValue
					? options.WithLanguageVersion(LanguageVersion.Value)
					: options;
			}
		}
	}
}
