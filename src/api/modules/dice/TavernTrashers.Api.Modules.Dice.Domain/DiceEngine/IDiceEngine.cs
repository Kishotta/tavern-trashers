using TavernTrashers.Api.Modules.Dice.Domain.Rolls;

namespace TavernTrashers.Api.Modules.Dice.Domain.DiceEngine;

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
	DieResult Roll(int sides);
}