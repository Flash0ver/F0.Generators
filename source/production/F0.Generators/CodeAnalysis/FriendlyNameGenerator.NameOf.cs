using System.CodeDom.Compiler;
using System.Diagnostics;
using F0.CodeDom.Compiler;
using F0.Text;

namespace F0.CodeAnalysis;

internal partial class FriendlyNameGenerator
{
	private const string NameOf_FieldName = "nameOf";
	private const string NameOf_MethodName = "NameOf";

	private static readonly SymbolDisplayFormat nameOfFormat = Create_NameOf_Format();

	private static IReadOnlyCollection<ITypeSymbol> GetDistinct_NameOf_Invocations(IReadOnlyCollection<TypeSyntax> syntaxes, Compilation compilation, CancellationToken cancellationToken)
	{
		HashSet<ITypeSymbol> types = new(SymbolEqualityComparer.Default);

		foreach (TypeSyntax syntax in syntaxes)
		{
			SemanticModel semanticModel = compilation.GetSemanticModel(syntax.SyntaxTree);
			ITypeSymbol? type = semanticModel.GetTypeInfo(syntax, cancellationToken).Type;

			if (type is null or IErrorTypeSymbol)
			{
				continue;
			}

			if (type.IsTupleType)
			{
				var namedType = type as INamedTypeSymbol;
				Debug.Assert(namedType is not null);

				type = namedType.TupleUnderlyingType ?? type;
			}

			if (type.SpecialType != SpecialType.System_Void)
			{
				_ = types.Add(type);
			}
		}

		return types;
	}

	private static void Write_NameOf_FieldDeclaration_To(IndentedTextWriter writer, IReadOnlyCollection<ITypeSymbol> types)
	{
		if (types.Count > 0)
		{
			writer.WriteLine($"private static readonly global::System.Collections.Generic.Dictionary<global::System.Type, string> {NameOf_FieldName} = Create{NameOf_MethodName}Lookup();");
		}
	}

	private static void Write_NameOf_MethodDeclaration_To(IndentedTextWriter writer, IReadOnlyCollection<ITypeSymbol> types)
	{
		writer.WriteLine($"public static string {NameOf_MethodName}<T>()");
		writer.WriteLine(Tokens.OpenBrace);
		writer.Indent++;

		if (types.Count > 0)
		{
			writer.WriteLine($"return {NameOf_FieldName}[typeof(T)];");
		}
		else
		{
			writer.WriteLine(Throws.Alias.NotGenerated);
		}

		writer.Indent--;
		writer.WriteLine(Tokens.CloseBrace);
	}

	private static void Write_CreateNameOf_MethodDeclaration_To(IndentedTextWriter writer, IReadOnlyCollection<ITypeSymbol> types, LanguageFeatures features)
	{
		if (types.Count > 0)
		{
			writer.WriteLineNoTabs();

			writer.WriteLine($"private static global::System.Collections.Generic.Dictionary<global::System.Type, string> Create{NameOf_MethodName}Lookup()");
			writer.WriteLine(Tokens.OpenBrace);
			writer.Indent++;

			CollectionInitializerExpression(writer, types, features);
			writer.Indent--;
			writer.WriteLine($"{Tokens.CloseBrace}");
		}

		static void CollectionInitializerExpression(IndentedTextWriter writer, IReadOnlyCollection<ITypeSymbol> types, LanguageFeatures features)
		{
			if (features.HasTargetTypedObjectCreation)
			{
				writer.WriteLine($"return new({types.Count})");
			}
			else
			{
				writer.WriteLine($"return new global::System.Collections.Generic.Dictionary<global::System.Type, string>({types.Count})");
			}

			writer.WriteLine(Tokens.OpenBrace);
			writer.Indent++;

			foreach (ITypeSymbol type in types)
			{
				writer.WriteLine($@"{{ typeof({type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}), ""{type.ToDisplayString(nameOfFormat)}"" }},");
			}

			writer.Indent--;
			writer.WriteLine($"{Tokens.CloseBrace}{Tokens.Semicolon}");
		}
	}

	private static SymbolDisplayFormat Create_NameOf_Format()
	{
		SymbolDisplayTypeQualificationStyle typeQualificationStyle = SymbolDisplayTypeQualificationStyle.NameAndContainingTypes;

		return new SymbolDisplayFormat(typeQualificationStyle: typeQualificationStyle)
			.AddGenericsOptions(SymbolDisplayGenericsOptions.IncludeTypeParameters)
			.AddMiscellaneousOptions(SymbolDisplayMiscellaneousOptions.UseSpecialTypes);
	}
}
