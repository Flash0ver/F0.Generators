using F0.Diagnostics;
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

		await VerifyAsync(test, null, generated, null);
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

		await VerifyAsync(test, null, generated, LanguageVersion.CSharp7_3);
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

		await VerifyAsync(test, null, generated, LanguageVersion.CSharp1);
	}

	[Fact]
	public async Task Execute_AvoidUsage()
	{
		string test =
@"#nullable enable
using System;
using System.Collections.Generic;
using F0.Generated;

public sealed class Class
{
	internal void Method(object obj, {|#0:SourceGenerationException?|} ex)
	{
		try
		{
			_ = new {|#1:SourceGenerationException|}("""");
			_ = obj as {|#2:SourceGenerationException|};
			_ = ({|#3:SourceGenerationException|})obj;
			_ = new List<{|#4:SourceGenerationException|}>(240);
		}
		catch ({|#5:SourceGenerationException|} exception)
		{
			throw new {|#6:SourceGenerationException|}("""");
		}
		catch (Exception exception) when (exception is {|#7:F0.Generated.SourceGenerationException|})
		{
			throw new {|#8:F0.Generated.SourceGenerationException|}("""");
		}
	}
}
";

		DiagnosticResult[] diagnostics = new[]
		{
			CreateDiagnostic(0, "SourceGenerationException? ex"),
			CreateDiagnostic(1, "new SourceGenerationException(\"\")"),
			CreateDiagnostic(2, "obj as SourceGenerationException"),
			CreateDiagnostic(3, "(SourceGenerationException)obj"),
			CreateDiagnostic(4, "<SourceGenerationException>"),
			CreateDiagnostic(5, "(SourceGenerationException exception)"),
			CreateDiagnostic(6, "throw new SourceGenerationException(\"\");"),
			CreateDiagnostic(7, "exception is F0.Generated.SourceGenerationException"),
			CreateDiagnostic(8, "throw new F0.Generated.SourceGenerationException(\"\");"),
		};

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

		await VerifyAsync(test, diagnostics, generated, null);
	}

	private static DiagnosticResult CreateDiagnostic(int markupKey, string expression)
	{
		return CSharpSourceGeneratorVerifier<SourceGenerationExceptionGenerator>.Diagnostic(DiagnosticIds.F0GEN0101, DiagnosticSeverity.Warning)
			.WithMessageFormat($"Avoid using 'SourceGenerationException' directly: '{{0}}'")
			.WithArguments(expression)
			.WithLocation(markupKey);
	}

	private static Task VerifyAsync(string test, DiagnosticResult[]? diagnostics, string generated, LanguageVersion? languageVersion)
	{
		diagnostics ??= Array.Empty<DiagnosticResult>();

		string filename = $@"F0.Generators\{typeof(SourceGenerationExceptionGenerator).FullName}\SourceGenerationException.g.cs";
		string content = String.Concat(Sources.GetFileHeader(languageVersion), generated);

		return CSharpSourceGeneratorVerifier<SourceGenerationExceptionGenerator>.VerifySourceGeneratorAsync(test, diagnostics, (filename, content), languageVersion);
	}
}
