using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading;
using F0.CodeDom.Compiler;
using F0.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace F0.CodeAnalysis
{
	internal partial class EnumInfoGenerator
	{
		internal const string MethodName = "GetName";

		private static readonly SymbolDisplayFormat fullyQualifiedFormat = CreateFullyQualifiedFormat();

		[System.Diagnostics.CodeAnalysis.SuppressMessage("MicrosoftCodeAnalysisCorrectness", "RS1024:Compare symbols correctly", Justification = "https://github.com/dotnet/roslyn-analyzers/issues/4568")]
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
					INamedTypeSymbol? typeSymbol = type as INamedTypeSymbol;
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
			{
				return unary.IsKind(SyntaxKind.BitwiseNotExpression);
			}
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
					Flags(writer, symbol, compilation.Options.CheckOverflow, fullyQualifiedName, features);
				}
				else
				{
					Enum(writer, symbol, compilation.Options.CheckOverflow, fullyQualifiedName, features);
				}

				writer.Indent--;
				writer.WriteLine(Tokens.CloseBrace);
			}

			static bool HasAttribute(INamedTypeSymbol symbol, INamedTypeSymbol attributeType)
			{
				return symbol.GetAttributes().Any(attribute => SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, attributeType));
			}

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

			static void Flags(IndentedTextWriter writer, INamedTypeSymbol symbol, bool checkOverflow, string fullyQualifiedName, LanguageFeatures features)
			{
				Debug.Assert(checkOverflow || !checkOverflow, "TODO");
				Debug.Assert(features is not null, "TODO");

				INamedTypeSymbol? underlyingType = symbol.EnumUnderlyingType;
				Debug.Assert(underlyingType is not null, $"{underlyingType} is not an enum type.");

				writer.WriteLine($"return ({underlyingType.ToDisplayString()})value switch");
				writer.WriteLine(Tokens.OpenBrace);
				writer.Indent++;

				EnumMember[] constants = symbol.GetMembers()
					.Where(static member => member.Kind is SymbolKind.Field)
					.Cast<IFieldSymbol>()
					.Select(field => new EnumMember(field))
					.OrderBy(member => member.Integral)
					.ToArray();

				EnumMember[] bitFlags = constants
					.Where(static constant => constant.Integral > BigInteger.Zero && IsPowerOfTwo(constant.Integral))
					.ToArray();

				foreach (EnumMember constant in constants)
				{
					if (constant.Integral > BigInteger.Zero)
					{
						writer.WriteLine($@"{constant.Integral} => ""{constant.Name}"",");

						if (IsPowerOfTwo(constant.Integral))
						{
							EnumMember[] range = bitFlags
								.Where(bitFlag => bitFlag.Integral < constant.Integral)
								.ToArray();

							BigInteger count = range.LongLength == 0L
								? BigInteger.Zero
								: (range[^1].Integral << 1) - BigInteger.One;

							for (BigInteger i = BigInteger.One; i <= count; i++)
							{
								EnumMember[] names = range
									.Where(bitFlag => (i & bitFlag.Integral) == bitFlag.Integral)
									.ToArray();

								BigInteger value = constant.Integral;

								foreach (EnumMember bitFlag in names)
								{
									value += bitFlag.Integral;
								}

								if (!constants.Any(constant => constant.Integral == value))
								{
									writer.Write(value);
									writer.Write(" => \"");

									foreach (EnumMember bitFlag in names)
									{
										writer.Write(bitFlag.Name);
										writer.Write(", ");
									}

									writer.Write(constant.Name);
									writer.WriteLine("\",");
								}
							}
						}
					}
					else
					{
						writer.WriteLine($@"{constant.Integral} => ""{constant.Name}"",");
					}
				}

				writer.WriteLine($"_ => throw new global::System.ComponentModel.InvalidEnumArgumentException(nameof(value), (int)value, typeof({fullyQualifiedName})),");

				writer.Indent--;
				writer.Write(Tokens.CloseBrace);
				writer.WriteLine(Tokens.Semicolon);
			}

			static bool IsImplicitlyConvertibleToInt32(INamedTypeSymbol type)
			{
				if (type.SpecialType is SpecialType.System_Byte
					or SpecialType.System_SByte
					or SpecialType.System_Int16
					or SpecialType.System_UInt16
					or SpecialType.System_Int32)
				{
					return true;
				}
				else
				{
					Debug.Assert(type.SpecialType is SpecialType.System_UInt32
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

		private static bool IsPowerOfTwo(BigInteger integral)
		{
			Debug.Assert(integral != BigInteger.Zero);

			return (integral & (integral - BigInteger.One)).IsZero;
		}

		private sealed class EnumMember
		{
			public EnumMember(IFieldSymbol constant)
			{
				Integral = ToIntegral(constant);
				Name = constant.Name;
			}

			public BigInteger Integral { get; }
			public string Name { get; }

			private static BigInteger ToIntegral(IFieldSymbol constant)
			{
				Debug.Assert(constant.HasConstantValue, $"{nameof(SymbolKind.Field)} {constant} is not a {nameof(constant)}.");

				return constant.ConstantValue switch
				{
					byte @byte => @byte,
					sbyte @sbyte => @sbyte,
					short @short => @short,
					ushort @ushort => @ushort,
					int @int => @int,
					uint @uint => @uint,
					long @long => @long,
					ulong @ulong => @ulong,
					_ => throw new ArgumentException($"Invalid {nameof(constant)} {constant}", nameof(constant)),
				};
			}
		}
	}
}
