using F0.Benchmarks.Measurers;
using F0.Shared;

namespace F0.Benchmarks.Shared;

public class SourceGenerationExceptionGeneratorBenchmarks
{
	private readonly CSharpSourceGeneratorMeasurer<SourceGenerationExceptionGenerator> benchmark = new();

	[GlobalSetup]
	public void Setup()
	{
		string code =
@"#nullable enable
using System;
using F0.Generated;

public sealed class Class
{
	public void Method()
	{
		throw new SourceGenerationException();
		throw new SourceGenerationException(""message"");
		throw new SourceGenerationException(""message"", new Exception());
	}
}
";

		benchmark.Initialize(code);
	}

	[Benchmark]
	public object? Generate()
	{
		benchmark.Invoke();
		return null;
	}

	[GlobalCleanup]
	public void Cleanup()
	{
		string generated =
$@"// <auto-generated/>

#nullable enable

namespace F0.Generated
{{
	internal sealed class SourceGenerationException : global::System.Exception
	{{
		private const string helpLink = ""https://github.com/Flash0ver/F0.Generators"";

		public SourceGenerationException()
			: base(CreateNotGeneratedMessage())
		{{
			HelpLink = helpLink;
		}}

		public SourceGenerationException(string message)
			: base(message)
		{{
			HelpLink = helpLink;
		}}

		public SourceGenerationException(string message, global::System.Exception innerException)
			: base(message, innerException)
		{{
			HelpLink = helpLink;
		}}

		private static string CreateNotGeneratedMessage()
		{{
			const string uri = ""https://github.com/Flash0ver/F0.Generators/issues"";

			return ""The method or operation was not generated correctly.""
				+ "" Please leave a comment on a related issue, or create a new issue at ""
				+ ""'"" + uri + ""'""
				+ "". Thank you!"";
		}}
	}}
}}
";

		benchmark.Inspect(generated);
	}
}
