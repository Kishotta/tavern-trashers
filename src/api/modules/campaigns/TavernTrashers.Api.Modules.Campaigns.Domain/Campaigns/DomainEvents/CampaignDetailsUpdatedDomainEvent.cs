namespace TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns;

public sealed class CampaignDetailsUpdatedDomainEvent(Guid campaignId) : DomainEvent
{
	public Guid CampaignId { get; } = campaignId;
}