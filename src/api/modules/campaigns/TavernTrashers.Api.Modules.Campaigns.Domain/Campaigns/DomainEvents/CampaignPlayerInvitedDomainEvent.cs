namespace TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns;

public sealed class CampaignPlayerInvitedDomainEvent(Guid campaignId, Guid playerId) : DomainEvent
{
	public Guid CampaignId { get; } = campaignId;
	public Guid PlayerId { get; } = playerId;
}