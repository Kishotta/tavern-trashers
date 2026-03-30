using TavernTrashers.Api.Common.Domain.Entities;

namespace TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns.Events;

public sealed class CampaignCreatedDomainEvent(Guid campaignId, string title) : DomainEvent
{
	public Guid CampaignId { get; } = campaignId;
	public string Title { get; } = title;
}
