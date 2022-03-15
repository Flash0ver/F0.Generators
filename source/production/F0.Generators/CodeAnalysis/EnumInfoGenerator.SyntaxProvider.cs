namespace F0.CodeAnalysis;

internal partial class EnumInfoGenerator
{
	private static bool SyntaxProviderPredicate(SyntaxNode syntaxNode, CancellationToken cancellationToken)
	{
		return syntaxNode is InvocationExpressionSyntax
		{
			ArgumentList.Arguments.Count: 1,
			Expression: MemberAccessExpressionSyntax
			{
				Name: IdentifierNameSyntax
				{
					Identifier.ValueText: MethodName
				}
			}
		};
	}

	private static InvocationExpressionSyntax SyntaxProviderTransform(GeneratorSyntaxContext context, CancellationToken cancellationToken)
	{
		var invocation = (InvocationExpressionSyntax)context.Node;
		return invocation;
	}
}
