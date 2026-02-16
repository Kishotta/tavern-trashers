using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Modules.Characters.Domain.Classes;

public static class CharacterClassErrors
{
	public static Error NotFound(Guid classId) =>
		Error.NotFound(
			"CharacterClasses.NotFound",
			$"Character class with ID '{classId}' not found.");

	public static Error InvalidName() =>
		Error.Validation(
			"CharacterClasses.InvalidName",
			"Character class name cannot be empty.");

	public static Error DuplicateName(string name) =>
		Error.Conflict(
			"CharacterClasses.DuplicateName",
			$"A character class with name '{name}' already exists.");
}
