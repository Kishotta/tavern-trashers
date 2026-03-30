using TavernTrashers.Api.Common.Application.EventBus;

namespace TavernTrashers.Api.Modules.Campaigns.IntegrationEvents;

public sealed class CampaignCreatedIntegrationEvent(
    Guid id,
    DateTime occurredAtUtc,
    Guid campaignId,
    string title)
    : IntegrationEvent(id, occurredAtUtc)
{
    public Guid CampaignId { get; } = campaignId;
    public string Title { get; } = title;
}
