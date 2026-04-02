using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Modules.Characters.Domain.Resources;

public static class SpellSlotPoolErrors
{
	public static Error NotFound(Guid id) =>
		Error.NotFound(
			"SpellSlotPool.NotFound",
			$"Spell slot pool with ID '{id}' was not found.");

	public static Error InvalidLevel(int level) =>
		Error.Validation(
			"SpellSlotPool.InvalidLevel",
			$"Spell slot level must be between 1 and 9. Got: {level}.");

	public static Error LevelEmpty(int level) =>
		Error.Conflict(
			"SpellSlotPool.LevelEmpty",
			$"No spell slots remaining at level {level}.");

	public static Error InvalidMaxUses(int max) =>
		Error.Validation(
			"SpellSlotPool.InvalidMaxUses",
			$"Max uses cannot be negative. Got: {max}.");

	public static Error PactMagicAlreadyExists() =>
		Error.Conflict(
			"SpellSlotPool.PactMagicAlreadyExists",
			"A Pact Magic slot pool already exists for this character.");
}
