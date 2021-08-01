using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace F0.CodeAnalysis
{
	internal sealed class EnumInfoReceiver : ISyntaxReceiver
	{
		internal static ISyntaxReceiver Create()
		{
			return new EnumInfoReceiver();
		}

		private readonly List<InvocationExpressionSyntax> invocations = new();

		private EnumInfoReceiver()
		{
		}

		internal IReadOnlyCollection<InvocationExpressionSyntax> Invocations => invocations;

		public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
		{
			if (syntaxNode.IsKind(SyntaxKind.InvocationExpression)
				&& syntaxNode is InvocationExpressionSyntax invocationExpression
				&& IsMatch(invocationExpression))
			{
				invocations.Add(invocationExpression);
			}
		}

		private static bool IsMatch(InvocationExpressionSyntax invocationExpression)
		{
			return invocationExpression is
			{
				ArgumentList:
				{
					Arguments:
					{
						Count: 1
					}
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
			};
		}
	}
}
