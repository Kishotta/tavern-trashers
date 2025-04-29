namespace TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns.DomainEvents;

public sealed class CampaignDetailsUpdatedDomainEvent(Guid campaignId) : DomainEvent
{
	public Guid CampaignId { get; } = campaignId;
}