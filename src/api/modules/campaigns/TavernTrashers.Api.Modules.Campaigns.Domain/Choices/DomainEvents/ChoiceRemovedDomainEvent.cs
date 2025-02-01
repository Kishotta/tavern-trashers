namespace TavernTrashers.Api.Modules.Campaigns.Domain.Choices.DomainEvents;

public  sealed class ChoiceRemovedDomainEvent(
	Guid questionnaireId,
	Guid questionId,
	Guid choiceId)
	: DomainEvent
{
	public Guid QuestionnaireId { get; } = questionnaireId;
	public Guid QuestionId { get; } = questionId;
	public Guid ChoiceId { get; } = choiceId;
}