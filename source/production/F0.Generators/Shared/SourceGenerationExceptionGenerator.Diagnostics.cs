using F0.Diagnostics;

namespace F0.Shared;

internal partial class SourceGenerationExceptionGenerator
{
	private static readonly DiagnosticDescriptor AvoidUsage = new(
		DiagnosticIds.F0GEN0101,
		$"Avoid using '{TypeName}' directly",
		$"Avoid using '{TypeName}' directly: '{{0}}'",
		DiagnosticCategories.SourceGenerationExceptionGenerator,
		DiagnosticSeverity.Warning,
		true,
		$"'{TypeName}' is intended for internal use by the generators to indicate generation errors or wrong usage.",
		DiagnosticHelpLinkUris.F0GEN0101
	);
}
