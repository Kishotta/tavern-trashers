using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Modules.Campaigns.Domain.Questionnaires;

public static class QuestionnaireErrors
{
	public static Error NotFound(Guid questionnaireId) =>
		Error.NotFound(
			"Questionnaire.NotFound",
			$"No questionnaire with identifier {questionnaireId} was found.");
}