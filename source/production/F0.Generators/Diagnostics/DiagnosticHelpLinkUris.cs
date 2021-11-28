namespace F0.Diagnostics;

internal static class DiagnosticHelpLinkUris
{
	private const string BaseAddress = "https://github.com/Flash0ver/F0.Generators/blob/main/docs/";
	private const string MarkdownExtension = ".md";
	private const string Anchor = "#";

	private const string SourceGenerationExceptionGenerator = nameof(SourceGenerationExceptionGenerator);
	private const string FriendlyNameGenerator = nameof(FriendlyNameGenerator);
	private const string EnumInfoGenerator = nameof(EnumInfoGenerator);

	internal const string F0GEN0101 = $"{BaseAddress}{nameof(SourceGenerationExceptionGenerator)}{MarkdownExtension}{Anchor}{nameof(F0GEN0101)}";

	internal const string F0GEN0301 = $"{BaseAddress}{nameof(EnumInfoGenerator)}{MarkdownExtension}{Anchor}{nameof(F0GEN0301)}";
	internal const string F0GEN0302 = $"{BaseAddress}{nameof(EnumInfoGenerator)}{MarkdownExtension}{Anchor}{nameof(F0GEN0302)}";
}
