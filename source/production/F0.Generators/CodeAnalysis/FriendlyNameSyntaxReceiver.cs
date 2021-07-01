using System;
using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace F0.CodeAnalysis
{
	internal sealed class FriendlyNameSyntaxReceiver : ISyntaxReceiver
	{
		internal static ISyntaxReceiver Create()
		{
			return new FriendlyNameSyntaxReceiver();
		}

		private FriendlyNameSyntaxReceiver()
		{
		}

		internal ImmutableArray<TypeSyntax> NameOfInvocations { get; private set; } = ImmutableArray<TypeSyntax>.Empty;
		internal ImmutableArray<TypeSyntax> FullNameOfInvocations { get; private set; } = ImmutableArray<TypeSyntax>.Empty;

		public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
		{
			if (syntaxNode is InvocationExpressionSyntax
				{
					Expression: MemberAccessExpressionSyntax
					{
						Name: GenericNameSyntax
						{
							TypeArgumentList:
							{
								Arguments:
								{
									Count: 1
								} arguments
							}
						} name
					},
					ArgumentList:
					{
						Arguments:
						{
							Count: 0
						}
					}
				})
			{
				Debug.Assert(arguments.Count == 1);

				if (name.Identifier.Text.Equals(FriendlyNameGenerator.NameOf_MethodName, StringComparison.Ordinal)
					&& arguments[0] is TypeSyntax nameOfInvocation)
				{
					Debug.Assert(nameOfInvocation is PredefinedTypeSyntax or NullableTypeSyntax or ArrayTypeSyntax or IdentifierNameSyntax or QualifiedNameSyntax or GenericNameSyntax or TupleTypeSyntax, $"{nameOfInvocation.Kind()}");

					NameOfInvocations = NameOfInvocations.Add(nameOfInvocation);
				}
				else if (name.Identifier.Text.Equals(FriendlyNameGenerator.FullNameOf_MethodName, StringComparison.Ordinal)
					&& arguments[0] is TypeSyntax fullNameOfInvocation)
				{
					Debug.Assert(fullNameOfInvocation is PredefinedTypeSyntax or NullableTypeSyntax or ArrayTypeSyntax or IdentifierNameSyntax or QualifiedNameSyntax or GenericNameSyntax or TupleTypeSyntax, $"{fullNameOfInvocation.Kind()}");

					FullNameOfInvocations = FullNameOfInvocations.Add(fullNameOfInvocation);
				}
			}
		}
	}
}
