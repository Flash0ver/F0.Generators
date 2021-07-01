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
		internal const string FullNameOf_FieldName = "fullNameOf";
		internal const string FullNameOf_MethodName = "FullNameOf";

		private static readonly SymbolDisplayFormat format = CreateFormat();

		private static IReadOnlyCollection<FriendlyFullNameOf> GetDistinctFullNameOfInvocations(IReadOnlyList<TypeSyntax> syntaxes, Compilation compilation, CancellationToken cancellationToken)
		{
			HashSet<FriendlyFullNameOf> types = new(FriendlyFullNameOfEqualityComparer.Instance);

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

				_ = types.Add(new FriendlyFullNameOf(type));
			}

			return types;
		}

		private static void Write_FullNameOf_FieldDeclaration_To(IndentedTextWriter writer, IReadOnlyCollection<FriendlyFullNameOf> types, LanguageFeatures features)
		{
			if (features.HasGenerics && types.Count > 0)
			{
				writer.WriteLine($"private static readonly global::System.Collections.Generic.Dictionary<global::System.Type, string> {FullNameOf_FieldName} = Create{FullNameOf_MethodName}Lookup();");
			}
		}

		private static void Write_FullNameOf_MethodDeclaration_To(IndentedTextWriter writer, IReadOnlyCollection<FriendlyFullNameOf> types, LanguageFeatures features)
		{
			if (features.HasGenerics)
			{
				writer.WriteLine($"public static string {FullNameOf_MethodName}<T>()");
			}
			else
			{
				writer.WriteLine($"public static string {FullNameOf_MethodName}()");
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
				writer.WriteLine($"return {FullNameOf_FieldName}[typeof(T)];");
			}
			else
			{
				writer.WriteLine(Throws.Alias.NotGenerated);
			}

			writer.Indent--;
			writer.WriteLine(Tokens.CloseBrace);
		}

		private static void Write_CreateFullNameOf_MethodDeclaration_To(IndentedTextWriter writer, IReadOnlyCollection<FriendlyFullNameOf> types, LanguageFeatures features)
		{
			if (features.HasGenerics && types.Count > 0)
			{
				writer.WriteLineNoTabs();

				writer.WriteLine($"private static global::System.Collections.Generic.Dictionary<global::System.Type, string> Create{FullNameOf_MethodName}Lookup()");
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

			static void ObjectCreationExpression(IndentedTextWriter writer, IReadOnlyCollection<FriendlyFullNameOf> types, LanguageFeatures features)
			{
				Debug.Assert(!features.HasImplicitlyTypedLocalVariable);
				writer.WriteLine($"global::System.Collections.Generic.Dictionary<global::System.Type, string> dictionary = new global::System.Collections.Generic.Dictionary<global::System.Type, string>({types.Count});");

				foreach (FriendlyFullNameOf type in types)
				{
					writer.WriteLine($@"dictionary.Add(typeof({type.Symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}), ""{type.Symbol.ToDisplayString(format)}"");");
				}

				writer.WriteLine($"return dictionary;");
			}

			static void CollectionInitializerExpression(IndentedTextWriter writer, IReadOnlyCollection<FriendlyFullNameOf> types, LanguageFeatures features)
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

				foreach (FriendlyFullNameOf type in types)
				{
					writer.WriteLine($@"{{ typeof({type.Symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}), ""{type.Symbol.ToDisplayString(format)}"" }},");
				}

				writer.Indent--;
				writer.WriteLine($"{Tokens.CloseBrace};");
			}
		}

		private sealed class FriendlyFullNameOf
		{
			public FriendlyFullNameOf(ITypeSymbol symbol)
			{
				Symbol = symbol;
			}

			public ITypeSymbol Symbol { get; }
		}

		private sealed class FriendlyFullNameOfEqualityComparer : EqualityComparer<FriendlyFullNameOf>
		{
			public static IEqualityComparer<FriendlyFullNameOf> Instance { get; } = new FriendlyFullNameOfEqualityComparer();

			private FriendlyFullNameOfEqualityComparer()
			{
			}

			public override bool Equals(FriendlyFullNameOf? x, FriendlyFullNameOf? y)
			{
				return SymbolEqualityComparer.Default.Equals(x?.Symbol, y?.Symbol);
			}

			public override int GetHashCode([DisallowNull] FriendlyFullNameOf obj)
			{
				return SymbolEqualityComparer.Default.GetHashCode(obj.Symbol);
			}
		}

		private static SymbolDisplayFormat CreateFormat()
		{
			SymbolDisplayTypeQualificationStyle typeQualificationStyle = SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces;

			return new SymbolDisplayFormat(typeQualificationStyle: typeQualificationStyle)
				.AddGenericsOptions(SymbolDisplayGenericsOptions.IncludeTypeParameters)
				.AddMiscellaneousOptions(SymbolDisplayMiscellaneousOptions.UseSpecialTypes);
		}
	}
}
