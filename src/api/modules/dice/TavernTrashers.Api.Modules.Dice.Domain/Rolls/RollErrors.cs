using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Modules.Dice.Domain.Rolls;

public static class RollErrors
{
	public static Error NotFound(Guid rollId) =>
		Error.NotFound(
			"Rolls.NotFound",
			$"Roll with ID '{rollId}' not found.");

	public static Error InvalidDiceIndex(int index, int count) =>
		Error.Validation(
			"Rolls.InvalidDiceIndex",
			$"The provided dice index '{index}' is out of range. Valid indices are from 0 to {count - 1}.");
}