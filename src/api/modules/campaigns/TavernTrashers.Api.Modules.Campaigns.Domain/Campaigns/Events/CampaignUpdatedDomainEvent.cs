using TavernTrashers.Api.Common.Domain.Entities;

namespace TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns.Events;

public sealed record CampaignUpdatedDomainEvent(Guid CampaignId, string Title) : DomainEvent;
