using F0.Diagnostics;

namespace F0.Shared;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0090:Use 'new(...)'", Justification = "https://github.com/dotnet/roslyn-analyzers/issues/5828")]
internal partial class SourceGenerationExceptionGenerator
{
	private static readonly DiagnosticDescriptor AvoidUsage = new DiagnosticDescriptor(
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
