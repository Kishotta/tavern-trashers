namespace TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns.DomainEvents;

public sealed class CampaignCreatedDomainEvent(Guid campaignId) : DomainEvent
{
	public Guid CampaignId { get; } = campaignId;
}