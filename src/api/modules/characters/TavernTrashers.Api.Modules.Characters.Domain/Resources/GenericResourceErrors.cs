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

	public static Error InvalidMaxUses() =>
		Error.Validation(
			"GenericResources.InvalidMaxUses",
			"Max uses cannot be negative.");

	public static Error AlreadyEmpty(Guid resourceId) =>
		Error.Validation(
			"GenericResources.AlreadyEmpty",
			$"Resource '{resourceId}' has no uses remaining.");

	public static Error AlreadyFull(Guid resourceId) =>
		Error.Validation(
			"GenericResources.AlreadyFull",
			$"Resource '{resourceId}' is already at maximum uses.");
}
