using System.CodeDom.Compiler;
using System.Diagnostics;
using F0.CodeDom.Compiler;
using F0.Text;

namespace F0.CodeAnalysis
{
	internal partial class FriendlyNameGenerator
	{
		internal const string NameOf_FieldName = "nameOf";
		internal const string NameOf_MethodName = "NameOf";

		private static readonly SymbolDisplayFormat nameOfFormat = Create_NameOf_Format();

		[System.Diagnostics.CodeAnalysis.SuppressMessage("MicrosoftCodeAnalysisCorrectness", "RS1024:Compare symbols correctly", Justification = "https://github.com/dotnet/roslyn-analyzers/issues/4568")]
		private static IReadOnlyCollection<ITypeSymbol> GetDistinct_NameOf_Invocations(IReadOnlyCollection<TypeSyntax> syntaxes, Compilation compilation, CancellationToken cancellationToken)
		{
			HashSet<ITypeSymbol> types = new(SymbolEqualityComparer.Default);

			foreach (TypeSyntax syntax in syntaxes)
			{
				SemanticModel semanticModel = compilation.GetSemanticModel(syntax.SyntaxTree);

				ITypeSymbol? type = semanticModel.GetTypeInfo(syntax, cancellationToken).Type;
				Debug.Assert(type is not null, $"Expression does not have a type: {syntax}");
				Debug.Assert(type is not IErrorTypeSymbol, $"Type could not be determined due to an error: {type}");

				if (type.IsTupleType)
				{
					var namedType = type as INamedTypeSymbol;
					Debug.Assert(namedType is not null);

					type = namedType.TupleUnderlyingType ?? type;
				}

				_ = types.Add(type);
			}

			return types;
		}

		private static void Write_NameOf_FieldDeclaration_To(IndentedTextWriter writer, IReadOnlyCollection<ITypeSymbol> types, LanguageFeatures features)
		{
			if (features.HasGenerics && types.Count > 0)
			{
				writer.WriteLine($"private static readonly global::System.Collections.Generic.Dictionary<global::System.Type, string> {NameOf_FieldName} = Create{NameOf_MethodName}Lookup();");
			}
		}

		private static void Write_NameOf_MethodDeclaration_To(IndentedTextWriter writer, IReadOnlyCollection<ITypeSymbol> types, LanguageFeatures features)
		{
			if (features.HasGenerics)
			{
				writer.WriteLine($"public static string {NameOf_MethodName}<T>()");
			}
			else
			{
				writer.WriteLine($"public static string {NameOf_MethodName}()");
			}
			writer.WriteLine(Tokens.OpenBrace);
			writer.Indent++;

			if (!features.HasGenerics)
			{
				Debug.Assert(!features.HasNamespaceAliasQualifier);
				writer.WriteLine(Throws.NotSupported(features.LanguageVersion, LanguageVersion.CSharp2));
			}
			else if (types.Count > 0)
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
			if (features.HasGenerics && types.Count > 0)
			{
				writer.WriteLineNoTabs();

				writer.WriteLine($"private static global::System.Collections.Generic.Dictionary<global::System.Type, string> Create{NameOf_MethodName}Lookup()");
				writer.WriteLine(Tokens.OpenBrace);
				writer.Indent++;

				if (features.HasCollectionInitializer)
				{
					CollectionInitializerExpression(writer, types, features);
				}
				else
				{
					ObjectCreationExpression(writer, types, features);
				}

				writer.Indent--;
				writer.WriteLine($"{Tokens.CloseBrace}");
			}

			static void ObjectCreationExpression(IndentedTextWriter writer, IReadOnlyCollection<ITypeSymbol> types, LanguageFeatures features)
			{
				Debug.Assert(!features.HasImplicitlyTypedLocalVariable);
				writer.WriteLine($"global::System.Collections.Generic.Dictionary<global::System.Type, string> dictionary = new global::System.Collections.Generic.Dictionary<global::System.Type, string>({types.Count});");

				foreach (ITypeSymbol type in types)
				{
					writer.WriteLine($@"dictionary.Add(typeof({type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}), ""{type.ToDisplayString(nameOfFormat)}"");");
				}

				writer.WriteLine($"return dictionary;");
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
}
