using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Modules.Dice.Domain.Rolls;

namespace TavernTrashers.Api.Modules.Dice.Domain.AbstractSyntaxTree;

/// <summary>
///     A "dX" roll with options
/// </summary>
/// <param name="count">The number of dice to roll</param>
/// <param name="sides">The number of sides on each dice</param>
/// <param name="explode">Whether to explode</param>
/// <param name="mode">Keep/Drop Mode</param>
/// <param name="modeCount">The number of dice to keep/drop</param>
public record DiceRollNode(
	int count,
	int sides,
	bool explode,
	KeepDropMode mode,
	int modeCount)
	: IExpressionNode
{
	public Result<RollOutcome> Evaluate(IDiceEngine diceEngine)
	{
		var rawRolls  = RollDice(diceEngine);
		var keptRolls = KeepDropDice(rawRolls);

		var actualTotal        = keptRolls.Sum();
		var theoreticalMinimum = keptRolls.Count * (sides == 0 ? -1 : 1);
		var theoreticalMaximum = keptRolls.Count * (sides == 0 ? 1 : sides);
		var theoreticalAverage = CalculateTheoreticalAverage(keptRolls);

		return new RollOutcome(
			actualTotal,
			theoreticalMinimum,
			theoreticalMaximum,
			theoreticalAverage,
			rawRolls,
			keptRolls
		);
	}

	private List<int> RollDice(IDiceEngine diceEngine)
	{
		var rawRolls = new List<int>();
		for (var i = 0; i < count; i++)
		{
			var roll = diceEngine.Roll(sides);
			rawRolls.Add(roll);
			// handle explode
#pragma warning disable S127
			if (explode && roll == sides) i--; // roll again
#pragma warning restore S127
		}

		return rawRolls;
	}

	private List<int> KeepDropDice(List<int> raw) =>
		mode switch
		{
			KeepDropMode.KeepHighest => raw.OrderByDescending(x => x).Take(modeCount).ToList(),
			KeepDropMode.KeepLowest  => raw.OrderBy(x => x).Take(modeCount).ToList(),
			KeepDropMode.DropHighest => raw.OrderByDescending(x => x).Skip(modeCount).ToList(),
			KeepDropMode.DropLowest  => raw.OrderBy(x => x).Skip(modeCount).ToList(),
			_                        => raw,
		};

	private double CalculateTheoreticalAverage(List<int> kept)
	{
		// rough average: sum of each die's expected value
		var perDieAverage = sides switch
		{
			0 => 0.0,
			_ => (1 + (double)sides) / 2.0,
		};

		return mode switch
		{
			KeepDropMode.None => count * perDieAverage,
			_                 => kept.Count * perDieAverage,
		};
	}
}