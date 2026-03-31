using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Modules.Characters.Domain.Resources;

public static class HitPointsErrors
{
	public static Error InvalidBaseMaxHitPoints(int value) =>
		Error.Validation(
			"HitPoints.InvalidBaseMaxHitPoints",
			$"Base max hit points '{value}' is invalid. Must be 0 or greater.");

	public static Error InvalidAmount(int value) =>
		Error.Validation(
			"HitPoints.InvalidAmount",
			$"Amount '{value}' is invalid. Must be 0 or greater.");

	public static Error InvalidTemporaryHitPoints(int value) =>
		Error.Validation(
			"HitPoints.InvalidTemporaryHitPoints",
			$"Temporary hit points '{value}' is invalid. Must be 0 or greater.");

	public static Error InvalidCurrentHitPoints(int value) =>
		Error.Validation(
			"HitPoints.InvalidCurrentHitPoints",
			$"Current hit points '{value}' is invalid. Must be 0 or greater.");

	public static Error InvalidMaxHitPointReduction(int value) =>
		Error.Validation(
			"HitPoints.InvalidMaxHitPointReduction",
			$"Max hit point reduction '{value}' is invalid. Must be greater than 0.");

	public static Error InvalidMaxHitPointReductionValue(int value) =>
		Error.Validation(
			"HitPoints.InvalidMaxHitPointReductionValue",
			$"Max hit point reduction '{value}' is invalid. Must be 0 or greater.");
}
