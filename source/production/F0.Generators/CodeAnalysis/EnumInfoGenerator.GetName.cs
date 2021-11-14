using System.CodeDom.Compiler;
using System.Diagnostics;
using F0.CodeDom.Compiler;
using F0.Text;

namespace F0.CodeAnalysis;

internal partial class EnumInfoGenerator
{
	internal const string MethodName = "GetName";

	private static readonly SymbolDisplayFormat fullyQualifiedFormat = CreateFullyQualifiedFormat();

	private static IReadOnlyCollection<INamedTypeSymbol> Get_GetName_Symbols(IReadOnlyCollection<ExpressionSyntax> arguments, Compilation compilation, CancellationToken cancellationToken)
	{
		HashSet<INamedTypeSymbol> symbols = new(SymbolEqualityComparer.Default);

		foreach (ExpressionSyntax argument in arguments)
		{
			SyntaxNode node = GetNode(argument);
			SemanticModel semanticModel = compilation.GetSemanticModel(argument.SyntaxTree);

			TypeInfo typeInfo = semanticModel.GetTypeInfo(node, cancellationToken);
			ITypeSymbol? type = typeInfo.Type;
			Debug.Assert(type is not null, $"Expression does not have a type: {node}");
			Debug.Assert(type is not IErrorTypeSymbol, $"Type could not be determined due to an error: {type}");

			if (type.TypeKind is TypeKind.Enum)
			{
				var typeSymbol = type as INamedTypeSymbol;
				Debug.Assert(typeSymbol is not null, $"Expected: {nameof(INamedTypeSymbol)} | Actual: {type}");

				_ = symbols.Add(typeSymbol);
			}
		}

		return symbols;

		static SyntaxNode GetNode(ExpressionSyntax expression)
		{
			SyntaxNode? node = expression switch
			{
				IdentifierNameSyntax name => name,
				MemberAccessExpressionSyntax { Name: IdentifierNameSyntax name } => name,
				CastExpressionSyntax cast => cast.Type,
				BinaryExpressionSyntax binary when CheckBinary(binary) => binary,
				PrefixUnaryExpressionSyntax unary when CheckUnary(unary) => unary,
				_ => null,
			};

			Debug.Assert(node is not null, $"Unexpected argument expression of {nameof(expression.Kind)} {expression.Kind()} : {expression}");

			return node;
		}

		static bool CheckBinary(BinaryExpressionSyntax binary)
		{
			return binary.IsKind(SyntaxKind.BitwiseOrExpression)
				|| binary.IsKind(SyntaxKind.BitwiseAndExpression)
				|| binary.IsKind(SyntaxKind.ExclusiveOrExpression);
		}

		static bool CheckUnary(PrefixUnaryExpressionSyntax unary)
			=> unary.IsKind(SyntaxKind.BitwiseNotExpression);
	}

	private static void Write_GetName_To(IndentedTextWriter writer, LanguageFeatures features)
	{
		bool useGlobal = features.HasNamespaceAliasQualifier;

		if (features.HasNullableReferenceTypes)
		{
			writer.WriteLine($"public static string {MethodName}(global::System.Enum? value)");
		}
		else if (useGlobal)
		{
			writer.WriteLine($"public static string {MethodName}(global::System.Enum value)");
		}
		else
		{
			writer.WriteLine($"public static string {MethodName}(System.Enum value)");
		}
		writer.WriteLine(Tokens.OpenBrace);
		if (features.HasInterpolatedStrings)
		{
			writer.WriteLineIndented(@"throw new global::F0.Generated.SourceGenerationException($""Cannot use the unspecialized method, which serves as a placeholder for the generator. Enum-Type {value?.GetType().ToString() ?? ""<null>""} must be concrete to generate the allocation-free variant of Enum.ToString()."");");
		}
		else if (useGlobal)
		{
			Debug.Assert(!features.HasPatternMatching);
			Debug.Assert(!features.HasNullPropagatingOperator);
			writer.WriteLineIndented(@"throw new global::F0.Generated.SourceGenerationException(""Cannot use the unspecialized method, which serves as a placeholder for the generator. Enum-Type "" + (value == null ? ""<null>"" : value.GetType().ToString()) + "" must be concrete to generate the allocation-free variant of Enum.ToString()."");");
		}
		else
		{
			writer.WriteLineIndented(@"throw new F0.Generated.SourceGenerationException(""Cannot use the unspecialized method, which serves as a placeholder for the generator. Enum-Type "" + (value == null ? ""<null>"" : value.GetType().ToString()) + "" must be concrete to generate the allocation-free variant of Enum.ToString()."");");
		}
		writer.WriteLine(Tokens.CloseBrace);
	}

	private static void Write_GetName_To(IndentedTextWriter writer, IReadOnlyCollection<INamedTypeSymbol> symbols, Compilation compilation, LanguageFeatures features)
	{
		INamedTypeSymbol? flagsAttributeType = compilation.GetTypeByMetadataName("System.FlagsAttribute");
		Debug.Assert(flagsAttributeType is not null, "System.FlagsAttribute type can't be found.");

		foreach (INamedTypeSymbol symbol in symbols)
		{
			string fullyQualifiedName = features.HasNamespaceAliasQualifier
				? symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
				: symbol.ToDisplayString(fullyQualifiedFormat.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted));

			writer.WriteLineNoTabs();

			writer.WriteLine($"public static string {MethodName}({fullyQualifiedName} value)");
			writer.WriteLine(Tokens.OpenBrace);
			writer.Indent++;

			if (HasAttribute(symbol, flagsAttributeType))
			{
				Flags(writer, features);
			}
			else
			{
				Enum(writer, symbol, compilation.Options.CheckOverflow, fullyQualifiedName, features);
			}

			writer.Indent--;
			writer.WriteLine(Tokens.CloseBrace);
		}

		static bool HasAttribute(INamedTypeSymbol symbol, INamedTypeSymbol attributeType)
			=> symbol.GetAttributes().Any(attribute => SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, attributeType));

		static void Enum(IndentedTextWriter writer, INamedTypeSymbol symbol, bool checkOverflow, string fullyQualifiedName, LanguageFeatures features)
		{
			bool useSwitchExpression = features.HasRecursivePatterns;
			bool useNameof = features.HasNameofOperator;
			bool useGlobal = features.HasNamespaceAliasQualifier;

			if (useSwitchExpression)
			{
				writer.WriteLine("return value switch");
			}
			else
			{
				writer.WriteLine("switch (value)");
			}
			writer.WriteLine(Tokens.OpenBrace);
			writer.Indent++;

			foreach (ISymbol member in symbol.GetMembers().Where(static member => member.Kind is SymbolKind.Field))
			{
				SymbolDisplayFormat format = useGlobal
					? fullyQualifiedFormat
					: fullyQualifiedFormat.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted);
				string constantValue = member.ToDisplayString(format);
				if (useSwitchExpression)
				{
					writer.WriteLine($"{constantValue} => nameof({constantValue}),");
				}
				else
				{
					writer.WriteLine($"case {constantValue}:");
					if (useNameof)
					{
						writer.WriteLineIndented($"return nameof({constantValue});");
					}
					else
					{
						writer.WriteLineIndented($@"return ""{member.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}"";");
					}
				}
			}

			INamedTypeSymbol? underlyingType = symbol.EnumUnderlyingType;
			Debug.Assert(underlyingType is not null, $"{underlyingType} is not an enum type.");

			string invalidValue = checkOverflow && !IsImplicitlyConvertibleToInt32(underlyingType) ? "unchecked((int)value)" : "(int)value";

			if (useSwitchExpression)
			{
				writer.WriteLine($"_ => throw new global::System.ComponentModel.InvalidEnumArgumentException(nameof(value), {invalidValue}, typeof({fullyQualifiedName})),");
			}
			else
			{
				writer.WriteLine("default:");
				if (useNameof)
				{
					writer.WriteLineIndented($"throw new global::System.ComponentModel.InvalidEnumArgumentException(nameof(value), {invalidValue}, typeof({fullyQualifiedName}));");
				}
				else if (useGlobal)
				{
					writer.WriteLineIndented($@"throw new global::System.ComponentModel.InvalidEnumArgumentException(""value"", {invalidValue}, typeof({fullyQualifiedName}));");
				}
				else
				{
					writer.WriteLineIndented($@"throw new System.ComponentModel.InvalidEnumArgumentException(""value"", {invalidValue}, typeof({fullyQualifiedName}));");
				}
			}

			writer.Indent--;
			if (useSwitchExpression)
			{
				writer.Write(Tokens.CloseBrace);
				writer.WriteLine(Tokens.Semicolon);
			}
			else
			{
				writer.WriteLine(Tokens.CloseBrace);
			}
		}

		static void Flags(IndentedTextWriter writer, LanguageFeatures features)
		{
			if (features.HasNamespaceAliasQualifier)
			{
				writer.WriteLine(@"throw new global::F0.Generated.SourceGenerationException(""Flags are not yet supported: see https://github.com/Flash0ver/F0.Generators/issues/1"");");
			}
			else
			{
				writer.WriteLine(@"throw new F0.Generated.SourceGenerationException(""Flags are not yet supported: see https://github.com/Flash0ver/F0.Generators/issues/1"");");
			}
		}

		static bool IsImplicitlyConvertibleToInt32(INamedTypeSymbol type)
		{
			if (type.SpecialType
				is SpecialType.System_Byte
				or SpecialType.System_SByte
				or SpecialType.System_Int16
				or SpecialType.System_UInt16
				or SpecialType.System_Int32)
			{
				return true;
			}
			else
			{
				Debug.Assert(type.SpecialType
					is SpecialType.System_UInt32
					or SpecialType.System_Int64
					or SpecialType.System_UInt64,
					$"Unhandled type {type}.");

				return false;
			}
		}
	}

	private static SymbolDisplayFormat CreateFullyQualifiedFormat()
	{
		SymbolDisplayGlobalNamespaceStyle globalNamespaceStyle = SymbolDisplayGlobalNamespaceStyle.Included;
		SymbolDisplayTypeQualificationStyle typeQualificationStyle = SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces;

		return new SymbolDisplayFormat(globalNamespaceStyle, typeQualificationStyle)
			.WithMemberOptions(SymbolDisplayMemberOptions.IncludeContainingType);
	}
}
