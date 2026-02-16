using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Modules.Characters.Domain.Characters;

public static class CharacterErrors
{
	public static Error NotFound(Guid characterId) =>
		Error.NotFound(
			"Characters.NotFound",
			$"Character with ID '{characterId}' not found.");

	public static Error InvalidName() =>
		Error.Validation(
			"Characters.InvalidName",
			"Character name cannot be empty.");

	public static Error InvalidLevel(int level) =>
		Error.Validation(
			"Characters.InvalidLevel",
			$"Level '{level}' is invalid. Must be between 1 and 20.");

	public static Error ClassLevelNotFound(Guid characterClassId) =>
		Error.NotFound(
			"Characters.ClassLevelNotFound",
			$"Character does not have a level in class '{characterClassId}'.");
}
