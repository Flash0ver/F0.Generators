using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Globalization;
using F0.CodeDom.Compiler;
using F0.Extensions;
using F0.Text;

namespace F0.CodeAnalysis;

[Generator]
internal sealed partial class EnumInfoGenerator : ISourceGenerator
{
	private const string TypeName = "EnumInfo";
	private const string HintName = "EnumInfo.g.cs";

	public void Initialize(GeneratorInitializationContext context)
		=> context.RegisterForSyntaxNotifications(EnumInfoReceiver.Create);

	public void Execute(GeneratorExecutionContext context)
	{
		Debug.Assert(context.SyntaxReceiver is not null);

		if (context.ParseOptions.IsCSharp() && context.SyntaxReceiver is EnumInfoReceiver receiver)
		{
			IReadOnlyCollection<INamedTypeSymbol> symbols = Get_GetName_Symbols(receiver.InvocationArguments, context.Compilation, context.CancellationToken);

			GeneratorOptions generatorOptions = GetOptions(context);
			string source = GenerateSourceCode(symbols, context.Compilation.Options, context.ParseOptions, generatorOptions);

			var sourceText = SourceText.From(source, Encodings.Utf8NoBom);
			context.AddSource(HintName, sourceText);
		}
	}

	private static GeneratorOptions GetOptions(GeneratorExecutionContext context)
	{
		return new GeneratorOptions
		{
			ThrowIfConstantNotFound = GetThrowIfConstantNotFound(context),
		};

		static bool GetThrowIfConstantNotFound(GeneratorExecutionContext context)
		{
			bool throwIfConstantNotFound;

			bool? config = null;
			if (context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("f0gen_enum_throw", out string? configSwitch))
			{
				config = configSwitch.Equals("true", StringComparison.OrdinalIgnoreCase)
					? true
					: configSwitch.Equals("false", StringComparison.OrdinalIgnoreCase)
						? false
						: null;
			}

			bool? build = null;
			if (context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.F0Gen_EnumInfo_ThrowIfConstantNotFound", out string? buildSwitch))
			{
				build = buildSwitch.Equals("true", StringComparison.OrdinalIgnoreCase) || buildSwitch.Equals("enable", StringComparison.OrdinalIgnoreCase)
					? true
					: buildSwitch.Equals("false", StringComparison.OrdinalIgnoreCase) || buildSwitch.Equals("disable", StringComparison.OrdinalIgnoreCase)
						? false
						: null;
			}

			if ((config is true && build is false) || (config is false && build is true))
			{
				throwIfConstantNotFound = false;
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
