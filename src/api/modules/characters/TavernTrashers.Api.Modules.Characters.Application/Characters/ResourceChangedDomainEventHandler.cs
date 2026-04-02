using TavernTrashers.Api.Common.Application.Hubs;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Modules.Characters.Application.Hubs;
using TavernTrashers.Api.Modules.Characters.Domain.Characters.Events;

namespace TavernTrashers.Api.Modules.Characters.Application.Characters;

internal sealed class ResourceChangedDomainEventHandler(IHubService hubService)
	: DomainEventHandler<ResourceChangedDomainEvent>
{
	public override Task Handle(
		ResourceChangedDomainEvent domainEvent,
		CancellationToken cancellationToken = default) =>
		hubService.PublishAsync(
			$"campaign:{domainEvent.CampaignId}",
			"ResourceChanged",
			new ResourceChangedNotification(
				domainEvent.CharacterId,
				domainEvent.CharacterName,
				domainEvent.CampaignId,
				domainEvent.ResourceName,
				domainEvent.OldValue,
				domainEvent.NewValue,
				domainEvent.Actor),
			cancellationToken);
}
