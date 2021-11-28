using System.Diagnostics;

namespace F0.CodeAnalysis;

internal sealed class EnumInfoReceiver : ISyntaxReceiver
{
	internal static ISyntaxReceiver Create()
		=> new EnumInfoReceiver();

	private readonly List<EnumInfoGetNameInvocation> invocations = new();

	private EnumInfoReceiver()
	{ }

	internal IReadOnlyCollection<EnumInfoGetNameInvocation> Invocations => invocations;

	public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
	{
		if (syntaxNode is InvocationExpressionSyntax
			{
				ArgumentList:
				{
					Arguments:
					{
						Count: 1
					} arguments
				},
				Expression: MemberAccessExpressionSyntax
				{
					Name: IdentifierNameSyntax
					{
						Identifier:
						{
							ValueText: EnumInfoGenerator.MethodName
						}
					}
				}
			} invocation)
		{
			ExpressionSyntax expression = GetArgumentExpression(arguments);

			invocations.Add(new EnumInfoGetNameInvocation(invocation, expression));
		}
	}

	private static ExpressionSyntax GetArgumentExpression(SeparatedSyntaxList<ArgumentSyntax> arguments)
	{
		Debug.Assert(arguments.Count == 1);

		ExpressionSyntax expression = arguments[0].Expression;

		if (expression is PostfixUnaryExpressionSyntax unary)
		{
			expression = unary.Operand;
		}

		return expression;
	}
}

internal sealed class EnumInfoGetNameInvocation
{
	public EnumInfoGetNameInvocation(InvocationExpressionSyntax invocation, ExpressionSyntax argument)
	{
		Invocation = invocation;
		Argument = argument;
	}

	public InvocationExpressionSyntax Invocation { get; }
	public ExpressionSyntax Argument { get; }
}
