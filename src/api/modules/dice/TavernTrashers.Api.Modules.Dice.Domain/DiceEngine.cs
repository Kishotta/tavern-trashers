namespace TavernTrashers.Api.Modules.Dice.Domain;

public interface IDiceEngine
{
	/// <summary>
	///     Roll a single die with <paramref name="sides">Sides</paramref> sides.
	/// </summary>
	/// <remarks>
	///     If sides == 0, you're doing a Fate die: return -1, 0, or 1.
	/// </remarks>
	/// <param name="sides"></param>
	/// <returns></returns>
	int Roll(int sides);
}

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

public record Roll(
	int Total,
	int Minimum,
	int Maximum,
	double Average,
	IReadOnlyList<int> RawRolls,
	IReadOnlyList<int> KeptRolls)
{
	public static Roll Constant(int value) =>
		new(value,
			value,
			value,
			value,
			[],
			[]);
}