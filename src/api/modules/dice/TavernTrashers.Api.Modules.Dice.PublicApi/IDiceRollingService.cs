using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Modules.Dice.Domain.Rolls;

namespace TavernTrashers.Api.Modules.Dice.PublicApi;

public interface IDiceRollingService
{
	Result<RollOutcome> RollDice(string expression);
	Result<int> RollInitiative(int modifier = 0);
}

public record DiceRollResult(
	int Total,
	IReadOnlyList<DieResult> RawRolls,
	IReadOnlyList<DieResult> KeptRolls,
	int Minimum,
	int Maximum,
	double Average);

public static class DiceRollingExtensions
{
	public static DiceRollResult ToPublicResult(this RollOutcome outcome) =>
		new(
			outcome.Total,
			outcome.RawRolls,
			outcome.KeptRolls,
			outcome.Minimum,
			outcome.Maximum,
			outcome.Average);
}