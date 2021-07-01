using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using F0.CodeDom.Compiler;
using F0.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace F0.CodeAnalysis
{
	internal partial class FriendlyNameGenerator
	{
		internal const string NameOf_FieldName = "nameOf";
		internal const string NameOf_MethodName = "NameOf";

		private static IReadOnlyCollection<FriendlyNameOf> GetDistinctNameOfInvocations(IReadOnlyList<TypeSyntax> syntaxes, Compilation compilation, CancellationToken cancellationToken)
		{
			HashSet<FriendlyNameOf> types = new(FriendlyNameOfEqualityComparer.Instance);

			foreach (TypeSyntax syntax in syntaxes)
			{
				SemanticModel semanticModel = compilation.GetSemanticModel(syntax.SyntaxTree);

				ITypeSymbol? type = semanticModel.GetTypeInfo(syntax, cancellationToken).Type;
				Debug.Assert(type is not null);
				Debug.Assert(type is not IErrorTypeSymbol, $"Type could not be determined due to an error: {type}");

				if (type.IsTupleType)
				{
					INamedTypeSymbol? namedType = type as INamedTypeSymbol;
					Debug.Assert(namedType is not null);

					type = namedType.TupleUnderlyingType ?? type;
				}

				_ = types.Add(new FriendlyNameOf(type, semanticModel, syntax.SpanStart));
			}

			return types;
		}

		private static void Write_NameOf_FieldDeclaration_To(IndentedTextWriter writer, IReadOnlyCollection<FriendlyNameOf> types, LanguageFeatures features)
		{
			if (features.HasGenerics && types.Count > 0)
			{
				writer.WriteLine($"private static readonly global::System.Collections.Generic.Dictionary<global::System.Type, string> {NameOf_FieldName} = Create{NameOf_MethodName}Lookup();");
			}
		}

		private static void Write_NameOf_MethodDeclaration_To(IndentedTextWriter writer, IReadOnlyCollection<FriendlyNameOf> types, LanguageFeatures features)
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

		private static void Write_CreateNameOf_MethodDeclaration_To(IndentedTextWriter writer, IReadOnlyCollection<FriendlyNameOf> types, LanguageFeatures features)
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

			static void ObjectCreationExpression(IndentedTextWriter writer, IReadOnlyCollection<FriendlyNameOf> types, LanguageFeatures features)
			{
				Debug.Assert(!features.HasImplicitlyTypedLocalVariable);
				writer.WriteLine($"global::System.Collections.Generic.Dictionary<global::System.Type, string> dictionary = new global::System.Collections.Generic.Dictionary<global::System.Type, string>({types.Count});");

				foreach (FriendlyNameOf type in types)
				{
					writer.WriteLine($@"dictionary.Add(typeof({type.Symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}), ""{type.Symbol.ToMinimalDisplayString(type.SemanticModel, type.Position)}"");");
				}

				writer.WriteLine($"return dictionary;");
			}

			static void CollectionInitializerExpression(IndentedTextWriter writer, IReadOnlyCollection<FriendlyNameOf> types, LanguageFeatures features)
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

				foreach (FriendlyNameOf type in types)
				{
					writer.WriteLine($@"{{ typeof({type.Symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}), ""{type.Symbol.ToMinimalDisplayString(type.SemanticModel, type.Position)}"" }},");
				}

				writer.Indent--;
				writer.WriteLine($"{Tokens.CloseBrace};");
			}
		}

		private sealed class FriendlyNameOf
		{
			public FriendlyNameOf(ITypeSymbol symbol, SemanticModel semanticModel, int position)
			{
				Symbol = symbol;
				SemanticModel = semanticModel;
				Position = position;
			}

			public ITypeSymbol Symbol { get; }
			public SemanticModel SemanticModel { get; }
			public int Position { get; }
		}

		private sealed class FriendlyNameOfEqualityComparer : EqualityComparer<FriendlyNameOf>
		{
			public static IEqualityComparer<FriendlyNameOf> Instance { get; } = new FriendlyNameOfEqualityComparer();

			private FriendlyNameOfEqualityComparer()
			{
			}

			public override bool Equals(FriendlyNameOf? x, FriendlyNameOf? y)
			{
				return SymbolEqualityComparer.Default.Equals(x?.Symbol, y?.Symbol);
			}

			public override int GetHashCode([DisallowNull] FriendlyNameOf obj)
			{
				return SymbolEqualityComparer.Default.GetHashCode(obj.Symbol);
			}
		}
	}
}
