using MediatR;
using TavernTrashers.Api.Common.Application.EventBus;
using TavernTrashers.Api.Modules.Campaigns.IntegrationEvents;
using TavernTrashers.Api.Modules.Characters.Application.Campaigns;

namespace TavernTrashers.Api.Modules.Characters.Presentation.Campaigns;

internal sealed class OnCampaignUpdated(ISender sender)
	: IntegrationEventHandler<CampaignUpdatedIntegrationEvent>
{
	public override async Task Handle(
		CampaignUpdatedIntegrationEvent integrationEvent,
		CancellationToken cancellationToken = default) =>
		await sender.Send(
			new UpdateCampaignReadModelCommand(integrationEvent.CampaignId, integrationEvent.Title),
			cancellationToken);
}
