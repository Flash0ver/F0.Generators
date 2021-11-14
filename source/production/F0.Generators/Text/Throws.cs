namespace F0.Text
{
	internal static class Throws
	{
		internal const string NotGenerated = "throw new F0.Generated.SourceGenerationException();";

		internal static string NotSupported(LanguageVersion currentLanguageVersion, LanguageVersion requiredLanguageVersion)
			=> $@"throw new F0.Generated.SourceGenerationException(""Feature is not available in C# {currentLanguageVersion.ToDisplayString()}. Please use language version {requiredLanguageVersion.ToDisplayString()} or greater."");";

		internal static class Alias
		{
			internal const string NotGenerated = "throw new global::F0.Generated.SourceGenerationException();";

			internal static string NotSupported(LanguageVersion currentLanguageVersion, LanguageVersion requiredLanguageVersion)
				=> $@"throw new global::F0.Generated.SourceGenerationException(""Feature is not available in C# {currentLanguageVersion.ToDisplayString()}. Please use language version {requiredLanguageVersion.ToDisplayString()} or greater."");";
		}
	}
}
