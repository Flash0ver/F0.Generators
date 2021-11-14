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

			string source = GenerateSourceCode(context.Compilation, context.ParseOptions, symbols);

			var sourceText = SourceText.From(source, Encodings.Utf8NoBom);
			context.AddSource(HintName, sourceText);
		}
	}

	private static string GenerateSourceCode(Compilation compilation, ParseOptions parseOptions, IReadOnlyCollection<INamedTypeSymbol> symbols)
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

		Write_GetName_To(source, languageFeatures);
		Write_GetName_To(source, symbols, compilation, languageFeatures);

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
}
