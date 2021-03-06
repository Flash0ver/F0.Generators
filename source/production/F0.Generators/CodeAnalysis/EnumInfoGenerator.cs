using System.CodeDom.Compiler;
using System.Collections.Immutable;
using System.Globalization;
using F0.CodeDom.Compiler;
using F0.Extensions;
using F0.Text;
using Microsoft.CodeAnalysis.Diagnostics;

namespace F0.CodeAnalysis;

[Generator]
internal sealed partial class EnumInfoGenerator : IIncrementalGenerator
{
	private const string TypeName = "EnumInfo";
	private const string HintName = "EnumInfo.g.cs";

	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		IncrementalValuesProvider<InvocationExpressionSyntax> syntaxProvider = context.SyntaxProvider
			.CreateSyntaxProvider(SyntaxProviderPredicate, SyntaxProviderTransform);

		IncrementalValueProvider<(ImmutableArray<InvocationExpressionSyntax> Invocations, Compilation Compilation)> withCompilation =
			syntaxProvider.Collect().Combine(context.CompilationProvider);

		IncrementalValueProvider<((ImmutableArray<InvocationExpressionSyntax> Invocations, Compilation Compilation) Other, ParseOptions ParseOptions)> withParseOptions =
			withCompilation.Combine(context.ParseOptionsProvider);

		IncrementalValueProvider<(((ImmutableArray<InvocationExpressionSyntax> Invocations, Compilation Compilation) Other, ParseOptions ParseOptions) Other, AnalyzerConfigOptionsProvider AnalyzerConfigOptions)> source =
			withParseOptions.Combine(context.AnalyzerConfigOptionsProvider);

		context.RegisterSourceOutput(source, Execute);
	}

	private static void Execute(SourceProductionContext context, (((ImmutableArray<InvocationExpressionSyntax> Invocations, Compilation Compilation) Other, ParseOptions ParseOptions) Other, AnalyzerConfigOptionsProvider AnalyzerConfigOptions) source)
	{
		if (source.Other.ParseOptions.IsCSharp())
		{
			IReadOnlyCollection<INamedTypeSymbol> symbols = Get_GetName_Symbols(source.Other.Other.Invocations, source.Other.Other.Compilation, context, context.CancellationToken);

			GeneratorOptions generatorOptions = GetOptions(source.AnalyzerConfigOptions, context);
			string text = GenerateSourceCode(symbols, source.Other.Other.Compilation.Options, source.Other.ParseOptions, generatorOptions);

			var sourceText = SourceText.From(text, Encodings.Utf8NoBom);
			context.AddSource(HintName, sourceText);
		}
	}

	private static GeneratorOptions GetOptions(AnalyzerConfigOptionsProvider analyzerConfigOptions, SourceProductionContext context)
	{
		return new GeneratorOptions
		{
			ThrowIfConstantNotFound = GetThrowIfConstantNotFound(analyzerConfigOptions, context),
		};

		static bool GetThrowIfConstantNotFound(AnalyzerConfigOptionsProvider analyzerConfigOptions, SourceProductionContext context)
		{
			bool throwIfConstantNotFound = false;

			bool? config = null;
			if (analyzerConfigOptions.GlobalOptions.TryGetValue("f0gen_enum_throw", out string? configSwitch))
			{
				config = configSwitch.Equals("true", StringComparison.OrdinalIgnoreCase)
					? true
					: configSwitch.Equals("false", StringComparison.OrdinalIgnoreCase)
						? false
						: null;
			}

			bool? build = null;
			if (analyzerConfigOptions.GlobalOptions.TryGetValue("build_property.F0Gen_EnumInfo_ThrowIfConstantNotFound", out string? buildSwitch))
			{
				build = buildSwitch.Equals("true", StringComparison.OrdinalIgnoreCase) || buildSwitch.Equals("enable", StringComparison.OrdinalIgnoreCase)
					? true
					: buildSwitch.Equals("false", StringComparison.OrdinalIgnoreCase) || buildSwitch.Equals("disable", StringComparison.OrdinalIgnoreCase)
						? false
						: null;
			}

			if ((config is true && build is false) || (config is false && build is true))
			{
				var diagnostic = Diagnostic.Create(AmbiguousConfiguration, Location.None, configSwitch, buildSwitch);
				context.ReportDiagnostic(diagnostic);
			}
			else
			{
				throwIfConstantNotFound = config is true || build is true;
			}

			return throwIfConstantNotFound;
		}
	}

	private static string GenerateSourceCode(IReadOnlyCollection<INamedTypeSymbol> symbols, CompilationOptions compilationOptions, ParseOptions parseOptions, GeneratorOptions generatorOptions)
	{
		using StringWriter writer = new(CultureInfo.InvariantCulture);
		using IndentedTextWriter source = new(writer, Trivia.Tab);

		LanguageVersion languageVersion = parseOptions.GetCSharpLanguageVersion();
		LanguageFeatures languageFeatures = new(languageVersion);

		bool useStaticClass = languageFeatures.HasStaticClasses;

		source.WriteLine("// <auto-generated/>");
		source.WriteLine();

		if (languageFeatures.HasNullableReferenceTypes)
		{
			source.WriteLine("#nullable enable");
			source.WriteLine();
		}

		source.WriteLine("namespace F0.Generated");
		source.WriteLine(Tokens.OpenBrace);
		source.Indent++;

		if (useStaticClass)
		{
			source.WriteLine($"internal static class {TypeName}");
		}
		else
		{
			source.WriteLine($"internal class {TypeName}");
		}
		source.WriteLine(Tokens.OpenBrace);
		source.Indent++;

		if (!useStaticClass)
		{
			source.WriteLine($"private {TypeName}()");
			source.WriteLine(Tokens.OpenBrace);
			source.WriteLine(Tokens.CloseBrace);
			source.WriteLineNoTabs();
		}

		Write_GetName_To(source, languageFeatures, generatorOptions);
		Write_GetName_To(source, symbols, compilationOptions, languageFeatures, generatorOptions);

		source.Indent--;
		source.WriteLine(Tokens.CloseBrace);

		source.Indent--;
		source.WriteLine(Tokens.CloseBrace);

		return writer.ToString();
	}

	private sealed class LanguageFeatures
	{
		public LanguageFeatures(LanguageVersion version)
			=> Version = version;

		public LanguageVersion Version { get; }

		public bool HasNullableReferenceTypes => Version >= LanguageVersion.CSharp8;
		public bool HasRecursivePatterns => Version >= LanguageVersion.CSharp8;
		public bool HasPatternMatching => Version >= LanguageVersion.CSharp7;
		public bool HasNameofOperator => Version >= LanguageVersion.CSharp6;
		public bool HasInterpolatedStrings => Version >= LanguageVersion.CSharp6;
		public bool HasNullPropagatingOperator => Version >= LanguageVersion.CSharp6;
		public bool HasNamespaceAliasQualifier => Version >= LanguageVersion.CSharp2;
		public bool HasStaticClasses => Version >= LanguageVersion.CSharp2;
	}

	private sealed class GeneratorOptions
	{
		public bool ThrowIfConstantNotFound { get; init; }
	}
}
