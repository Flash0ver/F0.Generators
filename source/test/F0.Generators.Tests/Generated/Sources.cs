using F0.Text;

namespace F0.Tests.Generated;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1802:Use literals where appropriate", Justification = "The feature 'constant interpolated strings' is currently in Preview and *unsupported*.")]
internal static class Sources
{
	internal static readonly string FileHeader = "// <auto-generated/>" + Environment.NewLine + Environment.NewLine;

	internal static readonly string NullableFileHeader = "// <auto-generated/>" + Environment.NewLine + Environment.NewLine
		+ "#nullable enable" + Environment.NewLine + Environment.NewLine;

	internal static readonly string SourceGenerationException = $"namespace {Names.Namespace} {{"
		+ $"internal sealed class {Names.Exception} : System.Exception {{ }}"
		+ "}";

	internal static readonly string SourceGenerationException_String = $"namespace {Names.Namespace} {{"
		+ $"internal sealed class {Names.Exception} : System.Exception {{"
		+ $"public {Names.Exception}(string message) : base(message) {{ }}"
		+ "}}";

	internal static string GetFileHeader(LanguageVersion? languageVersion = null)
	{
		return languageVersion.GetValueOrDefault(LanguageVersion.Latest) >= LanguageVersion.CSharp8
			? NullableFileHeader
			: FileHeader;
	}
}
