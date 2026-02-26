using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Modules.Characters.Domain.Classes;

public static class ClassErrors
{
	public static Error NotFound(Guid classId) =>
		Error.NotFound(
			"Classes.NotFound",
			$"Class with ID '{classId}' not found.");

	public static Error InvalidName() =>
		Error.Validation(
			"Classes.InvalidName",
			"Class name cannot be empty.");

	public static Error DuplicateName(string name) =>
		Error.Conflict(
			"Classes.DuplicateName",
			$"A class with name '{name}' already exists.");
}
