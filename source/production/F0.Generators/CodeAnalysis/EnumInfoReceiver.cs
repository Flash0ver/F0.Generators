using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace F0.CodeAnalysis
{
	internal sealed class EnumInfoReceiver : ISyntaxReceiver
	{
		internal static ISyntaxReceiver Create()
			=> new EnumInfoReceiver();

		private readonly List<ExpressionSyntax> invocationArguments = new();

		private EnumInfoReceiver()
		{ }

		internal IReadOnlyCollection<ExpressionSyntax> InvocationArguments => invocationArguments;

		public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
		{
			if (syntaxNode is InvocationExpressionSyntax
				{
					ArgumentList:
					{
						Arguments:
						{
							Count: 1
						} args
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
				})
			{
				if (TryGetArgumentExpression(args, out ExpressionSyntax? expression))
				{
					invocationArguments.Add(expression);
				}
			}
		}

		public static bool TryGetArgumentExpression(SeparatedSyntaxList<ArgumentSyntax> args, [NotNullWhen(true)] out ExpressionSyntax? argument)
		{
			Debug.Assert(args.Count == 1);

			ExpressionSyntax expression = args[0].Expression;

			if (expression is PostfixUnaryExpressionSyntax unary)
			{
				expression = unary.Operand;
			}

			if (expression is LiteralExpressionSyntax literal && CheckLiteral(literal))
			{
				argument = null;
				return false;
			}

			argument = expression;
			return true;
		}

		private static bool CheckLiteral(LiteralExpressionSyntax literal)
			=> literal.IsKind(SyntaxKind.NullLiteralExpression);
	}
}
