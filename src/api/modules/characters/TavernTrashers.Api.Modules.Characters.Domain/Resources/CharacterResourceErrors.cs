using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Modules.Characters.Domain.Resources;

public static class CharacterResourceErrors
{
	public static Error NotFound(Guid resourceId) =>
		Error.NotFound(
			"CharacterResources.NotFound",
			$"Character resource with ID '{resourceId}' not found.");

	public static Error InsufficientResources(Guid resourceId, int current, int requested) =>
		Error.Validation(
			"CharacterResources.InsufficientResources",
			$"Resource '{resourceId}' has {current} remaining but {requested} were requested.");

	public static Error InvalidAmount() =>
		Error.Validation(
			"CharacterResources.InvalidAmount",
			"Amount must be greater than zero.");

	public static Error InvalidName() =>
		Error.Validation(
			"CharacterResources.InvalidName",
			"Resource name cannot be empty.");
}
