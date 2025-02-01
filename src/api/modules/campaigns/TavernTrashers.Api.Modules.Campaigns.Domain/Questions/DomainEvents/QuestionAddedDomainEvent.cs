namespace TavernTrashers.Api.Modules.Campaigns.Domain.Questions.DomainEvents;

public sealed class QuestionAddedDomainEvent(Guid questionnaireId, Guid questionId) : DomainEvent
{
	public Guid QuestionnaireId { get; } = questionnaireId;
	public Guid QuestionId { get; } = questionId;
}