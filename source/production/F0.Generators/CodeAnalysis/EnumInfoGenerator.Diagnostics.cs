using F0.Diagnostics;

namespace F0.CodeAnalysis;

internal partial class EnumInfoGenerator
{
	private static readonly DiagnosticDescriptor UnspecializedPlaceholder = new(
		DiagnosticIds.F0GEN0301,
		$"Do not use the unspecialized placeholder method of '{TypeName}.{MethodName}({nameof(Enum)})'",
		$"Do not use the unspecialized placeholder method of '{TypeName}.{MethodName}({nameof(Enum)})' with non-enum argument '{{0}}'",
		DiagnosticCategories.EnumInfoGenerator,
		DiagnosticSeverity.Error,
		true,
		$"'{TypeName}.{MethodName}({nameof(Enum)})' does not return.",
		DiagnosticHelpLinkUris.F0GEN0301
	);

	private static readonly DiagnosticDescriptor AmbiguousConfiguration = new(
		DiagnosticIds.F0GEN0302,
		$"Ambiguous configuration of '{nameof(EnumInfoGenerator)}'",
		$"Ambiguous configuration of '{nameof(EnumInfoGenerator)}': Global AnalyzerConfig: 'f0gen_enum_throw = {{0}}' -and- MSBuild Property: '<F0Gen_EnumInfo_ThrowIfConstantNotFound>{{1}}</F0Gen_EnumInfo_ThrowIfConstantNotFound>'",
		DiagnosticCategories.EnumInfoGenerator,
		DiagnosticSeverity.Warning,
		true,
		$"The globalconfig option 'f0gen_enum_throw' and the MSBuild property 'F0Gen_EnumInfo_ThrowIfConstantNotFound' are ambiguous.",
		DiagnosticHelpLinkUris.F0GEN0302
	);
}
