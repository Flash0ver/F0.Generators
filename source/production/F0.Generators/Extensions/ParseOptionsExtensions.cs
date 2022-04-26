using System.Diagnostics;

namespace F0.Extensions;

internal static class ParseOptionsExtensions
{
	internal static bool IsCSharp(this ParseOptions parseOptions)
		=> parseOptions is CSharpParseOptions;

	internal static LanguageVersion GetCSharpLanguageVersion(this ParseOptions parseOptions)
	{
		var cSharpParseOptions = (CSharpParseOptions)parseOptions;

		Debug.Assert(parseOptions.Language.Equals(LanguageNames.CSharp, StringComparison.Ordinal));

		return cSharpParseOptions.LanguageVersion;
	}
}
