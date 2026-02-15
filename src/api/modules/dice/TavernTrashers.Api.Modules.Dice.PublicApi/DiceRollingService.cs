using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Dice.Domain.DiceEngine;
using TavernTrashers.Api.Modules.Dice.Domain.RecursiveDescentParser;
using TavernTrashers.Api.Modules.Dice.Domain.Rolls;

namespace TavernTrashers.Api.Modules.Dice.PublicApi;

public class DiceRollingService(IDiceEngine diceEngine) : IDiceRollingService
{
	public Result<RollOutcome> RollDice(string expression) =>
		new DiceParser(expression).ParseExpression()
		   .Match(result => result.Evaluate(diceEngine), failure => failure.Error);

	public Result<int> RollInitiative(int modifier = 0)
	{
		var sign = modifier >= 0 ? "+" : "-";
		var expression = modifier == 0
			? "d20"
			: $"d20{sign}{modifier}";
		
		return RollDice(expression)
		   .Transform(rollOutcome => rollOutcome.Total);
	}
}