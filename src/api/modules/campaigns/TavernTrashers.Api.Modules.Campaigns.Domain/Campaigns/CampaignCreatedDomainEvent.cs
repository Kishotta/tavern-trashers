using TavernTrashers.Api.Common.Domain.Entities;

namespace TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns;

public sealed class CampaignCreatedDomainEvent(CampaignId campaignId) : DomainEvent
{
	public CampaignId CampaignId { get; } = campaignId;
}