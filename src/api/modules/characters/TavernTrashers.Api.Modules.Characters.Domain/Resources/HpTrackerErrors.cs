using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Modules.Characters.Domain.Resources;

public static class HpTrackerErrors
{
	public static Error InvalidBaseMaxHp() =>
		Error.Validation("HpTracker.InvalidBaseMaxHp", "Base Max HP must be non-negative.");

	public static Error InvalidDamage() =>
		Error.Validation("HpTracker.InvalidDamage", "Damage must be non-negative.");

	public static Error InvalidHealing() =>
		Error.Validation("HpTracker.InvalidHealing", "Healing must be non-negative.");

	public static Error InvalidTemporaryHp() =>
		Error.Validation("HpTracker.InvalidTemporaryHp", "Temporary HP must be non-negative.");

	public static Error InvalidMaxHpReduction() =>
		Error.Validation("HpTracker.InvalidMaxHpReduction", "Max HP Reduction must be non-negative.");

	public static Error InvalidCurrentHp() =>
		Error.Validation("HpTracker.InvalidCurrentHp", "Current HP must be non-negative.");

	public static Error NotFound(Guid characterId) =>
		Error.NotFound("HpTracker.NotFound", $"HP Tracker for character with ID '{characterId}' was not found.");
}
