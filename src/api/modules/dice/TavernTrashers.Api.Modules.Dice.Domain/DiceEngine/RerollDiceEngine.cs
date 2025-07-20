using TavernTrashers.Api.Modules.Dice.Domain.Rolls;

namespace TavernTrashers.Api.Modules.Dice.Domain.DiceEngine;

public class RerollDiceEngine(
	IList<DieResult> originalRawRolls,
	IEnumerable<int> rerollDiceIndices,
	IDiceEngine fallback) : IDiceEngine
{
	private int _callIndex;

	public DieResult Roll(int sides)
	{
		if (_callIndex >= originalRawRolls.Count)
			return fallback.Roll(sides);

		var diceIndex = _callIndex++;
		return rerollDiceIndices.Contains(diceIndex)
			? fallback.Roll(sides)
			: originalRawRolls[diceIndex];
	}
}