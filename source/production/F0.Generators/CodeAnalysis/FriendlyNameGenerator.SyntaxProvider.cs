using System.Diagnostics;

namespace F0.CodeAnalysis;

internal partial class FriendlyNameGenerator
{
	private static bool SyntaxProviderPredicate(SyntaxNode syntaxNode, string methodName)
	{
		return syntaxNode is InvocationExpressionSyntax
		{
			Expression: MemberAccessExpressionSyntax
			{
				Name: GenericNameSyntax
				{
					TypeArgumentList.Arguments:
					{
						Count: 1
					} arguments,
					Identifier.Text: string identifier
				}
			},
			ArgumentList.Arguments.Count: 0
		}
			&& identifier.Equals(methodName, StringComparison.Ordinal)
			&& arguments[0] is not OmittedTypeArgumentSyntax;
	}

	private static TypeSyntax SyntaxProviderTransform(GeneratorSyntaxContext context, CancellationToken cancellationToken)
	{
		var invocation = (InvocationExpressionSyntax)context.Node;
		var expression = (MemberAccessExpressionSyntax)invocation.Expression;
		var name = (GenericNameSyntax)expression.Name;
		SeparatedSyntaxList<TypeSyntax> arguments = name.TypeArgumentList.Arguments;

		Debug.Assert(arguments.Count == 1);

		TypeSyntax type = arguments[0];

		Debug.Assert(type is PredefinedTypeSyntax or NullableTypeSyntax or ArrayTypeSyntax or IdentifierNameSyntax or QualifiedNameSyntax or GenericNameSyntax or TupleTypeSyntax, $"{type.Kind()}");

		return type;
	}
}
