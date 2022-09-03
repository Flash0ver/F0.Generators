namespace F0.CodeAnalysis;

internal partial class SourceGenerationExceptionGenerator
{
	private static bool SyntaxProviderPredicate(SyntaxNode syntaxNode, CancellationToken cancellationToken)
	{
		return syntaxNode is IdentifierNameSyntax
		{
			Identifier.Text: TypeName
		};
	}

	private static IdentifierNameSyntax SyntaxProviderTransform(GeneratorSyntaxContext context, CancellationToken cancellationToken)
	{
		var name = (IdentifierNameSyntax)context.Node;
		return name;
	}
}
