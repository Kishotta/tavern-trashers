namespace TavernTrashers.Api.Modules.Dice.Domain.Rolls;

public class DefaultDiceEngine(int? seed = null) : IDiceEngine
{
	private readonly Random _rng = seed.HasValue
		? new(seed.Value)
		: new Random();

	public int Roll(int sides) =>
		sides == 0
			? _rng.Next(0, 3) - 1      // Fate: map 0,1,2 → –1,0,1
			: _rng.Next(1, sides + 1); // standard 1..sides
}