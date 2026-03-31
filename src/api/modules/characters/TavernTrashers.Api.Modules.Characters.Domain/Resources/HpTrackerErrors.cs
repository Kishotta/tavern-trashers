using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Modules.Characters.Domain.Resources;

public static class HpTrackerErrors
{
	public static Error InvalidBaseMaxHp(int value) =>
		Error.Validation(
			"HpTracker.InvalidBaseMaxHp",
			$"Base max HP '{value}' is invalid. Must be greater than 0.");

	public static Error InvalidAmount(int value) =>
		Error.Validation(
			"HpTracker.InvalidAmount",
			$"Amount '{value}' is invalid. Must be greater than 0.");

	public static Error InvalidTemporaryHp(int value) =>
		Error.Validation(
			"HpTracker.InvalidTemporaryHp",
			$"Temporary HP '{value}' is invalid. Must be 0 or greater.");

	public static Error InvalidCurrentHp(int value) =>
		Error.Validation(
			"HpTracker.InvalidCurrentHp",
			$"Current HP '{value}' is invalid. Must be 0 or greater.");

	public static Error InvalidMaxHpReduction(int value) =>
		Error.Validation(
			"HpTracker.InvalidMaxHpReduction",
			$"Max HP reduction '{value}' is invalid. Must be greater than 0.");

	public static Error NotFound(Guid characterId) =>
		Error.NotFound(
			"HpTracker.NotFound",
			$"HP tracker for character '{characterId}' has not been initialized.");
}
