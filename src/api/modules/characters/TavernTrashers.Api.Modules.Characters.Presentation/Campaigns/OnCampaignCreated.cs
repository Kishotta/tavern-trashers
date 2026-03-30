using MediatR;
using TavernTrashers.Api.Common.Application.EventBus;
using TavernTrashers.Api.Modules.Campaigns.IntegrationEvents;
using TavernTrashers.Api.Modules.Characters.Application.Campaigns;

namespace TavernTrashers.Api.Modules.Characters.Presentation.Campaigns;

internal sealed class OnCampaignCreated(ISender sender)
	: IntegrationEventHandler<CampaignCreatedIntegrationEvent>
{
	public override async Task Handle(
		CampaignCreatedIntegrationEvent integrationEvent,
		CancellationToken cancellationToken = default) =>
		await sender.Send(
			new CreateCampaignReadModelCommand(integrationEvent.CampaignId, integrationEvent.Title),
			cancellationToken);
}
