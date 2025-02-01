namespace TavernTrashers.Api.Modules.Campaigns.Domain.Questionnaires.DomainEvents;

public sealed class QuestionnaireCreatedDomainEvent(Guid questionnaireId) : DomainEvent
{
	public Guid QuestionnaireId { get; } = questionnaireId;
}