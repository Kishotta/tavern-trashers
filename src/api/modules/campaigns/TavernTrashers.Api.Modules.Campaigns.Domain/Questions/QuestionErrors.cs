using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Modules.Campaigns.Domain.Questions;

public static class QuestionErrors
{
	public static Error NotFound(Guid questionId) =>
		Error.NotFound(
			"Question.NotFound",
			$"No question with identifier {questionId} was found.");
	
	public static Error AlreadyExists =>
		Error.Conflict(
			"Question.AlreadyExists",
			"Question with the same prompt already exists in the questionnaire.");
}