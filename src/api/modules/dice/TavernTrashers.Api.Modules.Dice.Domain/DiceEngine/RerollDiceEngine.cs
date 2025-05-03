namespace TavernTrashers.Api.Modules.Dice.Domain.DiceEngine;

public class RerollDiceEngine(
	IList<int> originalRawRolls,
	IEnumerable<int> rerollDiceIndices,
	IDiceEngine fallback) : IDiceEngine
{
	private int _callIndex;

	public int Roll(int sides)
	{
		if (_callIndex >= originalRawRolls.Count)
			return fallback.Roll(sides);

		var idx = _callIndex++;
		return rerollDiceIndices.Contains(idx)
			? fallback.Roll(sides)
			: originalRawRolls[idx];
	}
}