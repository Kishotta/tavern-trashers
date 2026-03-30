using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Modules.Characters.Domain.Resources;

public static class GenericResourceErrors
{
	public static Error NotFound(Guid resourceId) =>
		Error.NotFound(
			"GenericResources.NotFound",
			$"Generic resource with ID '{resourceId}' not found.");

	public static Error InvalidName() =>
		Error.Validation(
			"GenericResources.InvalidName",
			"Resource name cannot be empty.");

	public static Error InvalidMaxAmount() =>
		Error.Validation(
			"GenericResources.InvalidMaxAmount",
			"Max amount cannot be negative.");

	public static Error InsufficientResources(Guid resourceId, int current) =>
		Error.Validation(
			"GenericResources.InsufficientResources",
			$"Resource '{resourceId}' has {current} remaining and cannot be decremented further.");

	public static Error ResourceExceeded(Guid resourceId, int current, int max) =>
		Error.Validation(
			"GenericResources.ResourceExceeded",
			$"Resource '{resourceId}' is at maximum ({current}/{max}) and cannot be incremented further.");
}
