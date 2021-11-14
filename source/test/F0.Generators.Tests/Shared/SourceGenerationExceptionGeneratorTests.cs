using F0.Shared;
using F0.Tests.Generated;
using F0.Tests.Verifiers;

namespace F0.Tests.Shared;

public class SourceGenerationExceptionGeneratorTests
{
	[Fact]
	public async Task Execute_Unconditionally()
	{
		string test = String.Empty;

		string generated =
@"namespace F0.Generated
{
	internal sealed class SourceGenerationException : global::System.Exception
	{
		private const string helpLink = ""https://github.com/Flash0ver/F0.Generators"";

		public SourceGenerationException()
			: base(CreateNotGeneratedMessage())
		{
			HelpLink = helpLink;
		}

		public SourceGenerationException(string message)
			: base(message)
		{
			HelpLink = helpLink;
		}

		public SourceGenerationException(string message, global::System.Exception innerException)
			: base(message, innerException)
		{
			HelpLink = helpLink;
		}

		private static string CreateNotGeneratedMessage()
		{
			const string uri = ""https://github.com/Flash0ver/F0.Generators/issues"";

			return ""The method or operation was not generated correctly.""
				+ "" Please leave a comment on a related issue, or create a new issue at ""
				+ ""'"" + uri + ""'""
				+ "". Thank you!"";
		}
	}
}
";

		await VerifyAsync(test, generated);
	}

	[Fact]
	public async Task Execute_Nullability_Oblivious()
	{
		string test = String.Empty;

		string generated =
@"namespace F0.Generated
{
	internal sealed class SourceGenerationException : global::System.Exception
	{
		private const string helpLink = ""https://github.com/Flash0ver/F0.Generators"";

		public SourceGenerationException()
			: base(CreateNotGeneratedMessage())
		{
			HelpLink = helpLink;
		}

		public SourceGenerationException(string message)
			: base(message)
		{
			HelpLink = helpLink;
		}

		public SourceGenerationException(string message, global::System.Exception innerException)
			: base(message, innerException)
		{
			HelpLink = helpLink;
		}

		private static string CreateNotGeneratedMessage()
		{
			const string uri = ""https://github.com/Flash0ver/F0.Generators/issues"";

			return ""The method or operation was not generated correctly.""
				+ "" Please leave a comment on a related issue, or create a new issue at ""
				+ ""'"" + uri + ""'""
				+ "". Thank you!"";
		}
	}
}
";

		await VerifyAsync(test, generated, LanguageVersion.CSharp7_3);
	}

	[Fact]
	public async Task Execute_NoGlobalNamespaceAlias()
	{
		string test = String.Empty;

		string generated =
@"namespace F0.Generated
{
	internal sealed class SourceGenerationException : System.Exception
	{
		private const string helpLink = ""https://github.com/Flash0ver/F0.Generators"";

		public SourceGenerationException()
			: base(CreateNotGeneratedMessage())
		{
			HelpLink = helpLink;
		}

		public SourceGenerationException(string message)
			: base(message)
		{
			HelpLink = helpLink;
		}

		public SourceGenerationException(string message, System.Exception innerException)
			: base(message, innerException)
		{
			HelpLink = helpLink;
		}

		private static string CreateNotGeneratedMessage()
		{
			const string uri = ""https://github.com/Flash0ver/F0.Generators/issues"";

			return ""The method or operation was not generated correctly.""
				+ "" Please leave a comment on a related issue, or create a new issue at ""
				+ ""'"" + uri + ""'""
				+ "". Thank you!"";
		}
	}
}
";

		await VerifyAsync(test, generated, LanguageVersion.CSharp1);
	}

	private static Task VerifyAsync(string test, string generated, LanguageVersion? languageVersion = null)
	{
		string filename = $@"F0.Generators\{typeof(SourceGenerationExceptionGenerator).FullName}\SourceGenerationException.g.cs";
		string content = String.Concat(Sources.GetFileHeader(languageVersion), generated);

		return CSharpSourceGeneratorVerifier<SourceGenerationExceptionGenerator>.VerifySourceGeneratorAsync(test, (filename, content), languageVersion);
	}
}
