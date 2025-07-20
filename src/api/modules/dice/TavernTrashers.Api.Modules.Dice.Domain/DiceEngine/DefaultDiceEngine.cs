using TavernTrashers.Api.Modules.Dice.Domain.Rolls;

namespace TavernTrashers.Api.Modules.Dice.Domain.DiceEngine;

public class DefaultDiceEngine(int? seed = null) : IDiceEngine
{
	private readonly Random _rng = seed.HasValue
		? new(seed.Value)
		: new Random();

	public DieResult Roll(int sides) =>
		sides == 0
			? new(_rng.Next(0, 3) - 1, "f")                   // Fate: map 0,1,2 → –1,0,1
			: new(_rng.Next(1, sides + 1), sides.ToString()); // standard 1..sides
}