using System.Diagnostics;

namespace F0.CodeAnalysis;

internal sealed class FriendlyNameReceiver : ISyntaxReceiver
{
	internal static ISyntaxReceiver Create()
		=> new FriendlyNameReceiver();

	private readonly List<TypeSyntax> nameOfInvocations = new();
	private readonly List<TypeSyntax> fullNameOfInvocations = new();

	private FriendlyNameReceiver()
	{ }

	internal IReadOnlyCollection<TypeSyntax> NameOfInvocations => nameOfInvocations;
	internal IReadOnlyCollection<TypeSyntax> FullNameOfInvocations => fullNameOfInvocations;

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
				&& arguments[0] is TypeSyntax nameOfInvocation
					and not OmittedTypeArgumentSyntax)
			{
				Debug.Assert(nameOfInvocation is PredefinedTypeSyntax or NullableTypeSyntax or ArrayTypeSyntax or IdentifierNameSyntax or QualifiedNameSyntax or GenericNameSyntax or TupleTypeSyntax, $"{nameOfInvocation.Kind()}");

				nameOfInvocations.Add(nameOfInvocation);
			}
			else if (name.Identifier.Text.Equals(FriendlyNameGenerator.FullNameOf_MethodName, StringComparison.Ordinal)
				&& arguments[0] is TypeSyntax fullNameOfInvocation
					and not OmittedTypeArgumentSyntax)
			{
				Debug.Assert(fullNameOfInvocation is PredefinedTypeSyntax or NullableTypeSyntax or ArrayTypeSyntax or IdentifierNameSyntax or QualifiedNameSyntax or GenericNameSyntax or TupleTypeSyntax, $"{fullNameOfInvocation.Kind()}");

				fullNameOfInvocations.Add(fullNameOfInvocation);
			}
		}
	}
}
