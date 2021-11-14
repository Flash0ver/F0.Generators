using System;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace F0.Extensions
{
	internal static class ParseOptionsExtensions
	{
		internal static bool IsCSharp(this ParseOptions parseOptions)
			=> parseOptions.Language.Equals(LanguageNames.CSharp, StringComparison.Ordinal);

		internal static LanguageVersion GetCSharpLanguageVersion(this ParseOptions parseOptions)
		{
			Debug.Assert(parseOptions is CSharpParseOptions);
			CSharpParseOptions cSharpParseOptions = (CSharpParseOptions)parseOptions;

			return cSharpParseOptions.LanguageVersion;
		}

		internal static bool IsCSharp2OrGreater(this ParseOptions parseOptions)
		{
			Debug.Assert(parseOptions is CSharpParseOptions);
			CSharpParseOptions cSharpParseOptions = (CSharpParseOptions)parseOptions;

			return cSharpParseOptions.LanguageVersion >= LanguageVersion.CSharp2;
		}

		internal static bool IsCSharp3OrGreater(this ParseOptions parseOptions)
		{
			Debug.Assert(parseOptions is CSharpParseOptions);
			CSharpParseOptions cSharpParseOptions = (CSharpParseOptions)parseOptions;

			return cSharpParseOptions.LanguageVersion >= LanguageVersion.CSharp3;
		}

		internal static bool IsCSharp8OrGreater(this ParseOptions parseOptions)
		{
			Debug.Assert(parseOptions is CSharpParseOptions);
			CSharpParseOptions cSharpParseOptions = (CSharpParseOptions)parseOptions;

			return cSharpParseOptions.LanguageVersion >= LanguageVersion.CSharp8;
		}

		internal static bool IsCSharp9OrGreater(this ParseOptions parseOptions)
		{
			Debug.Assert(parseOptions is CSharpParseOptions);
			CSharpParseOptions cSharpParseOptions = (CSharpParseOptions)parseOptions;

			return cSharpParseOptions.LanguageVersion >= LanguageVersion.CSharp9;
		}
	}
}
