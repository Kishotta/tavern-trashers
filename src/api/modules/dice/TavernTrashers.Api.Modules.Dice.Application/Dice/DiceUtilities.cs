using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Modules.Dice.Domain.AbstractSyntaxTree;
using TavernTrashers.Api.Modules.Dice.Domain.RecursiveDescentParser;

namespace TavernTrashers.Api.Modules.Dice.Application.Dice;

internal static class DiceUtilities
{
	internal static Result<IExpressionNode> ParseDiceExpression(this string diceExpression) =>
		new DiceParser(diceExpression)
		   .ParseExpression();
}