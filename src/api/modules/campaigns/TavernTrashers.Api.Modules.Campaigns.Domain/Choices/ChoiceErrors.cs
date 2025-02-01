using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Modules.Campaigns.Domain.Choices;

public static class ChoiceErrors
{
	public static Error NotFound(Guid choiceId) =>
		Error.NotFound(
			"Choice.NotFound",
			$"No choice with identifier {choiceId} was found.");
	
	public static Error AlreadyExists =>
		Error.Conflict(
			"Choice.AlreadyExist",
			$"Choice with the same value already exists for the question.");
}