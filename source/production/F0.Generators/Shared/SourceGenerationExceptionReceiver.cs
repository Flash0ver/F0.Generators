namespace F0.Shared;

internal sealed class SourceGenerationExceptionReceiver : ISyntaxReceiver
{
	internal static ISyntaxReceiver Create()
		=> new SourceGenerationExceptionReceiver();

	private readonly List<IdentifierNameSyntax> references = new();

	private SourceGenerationExceptionReceiver()
	{ }

	public IReadOnlyCollection<IdentifierNameSyntax> References => references;

	public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
	{
		if (syntaxNode is IdentifierNameSyntax
			{
				Identifier:
				{
					Text: SourceGenerationExceptionGenerator.TypeName,
				}
			} name)
		{
			references.Add(name);
		}
	}
}
