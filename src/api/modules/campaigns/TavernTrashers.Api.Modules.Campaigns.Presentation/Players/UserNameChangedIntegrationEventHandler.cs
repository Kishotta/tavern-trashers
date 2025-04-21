using TavernTrashers.Api.Common.Application.EventBus;
using TavernTrashers.Api.Common.Application.Exceptions;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Campaigns.Application.Players;
using TavernTrashers.Api.Modules.Users.IntegrationEvents;

namespace TavernTrashers.Api.Modules.Campaigns.Presentation.Players;

public sealed class UserNameChangedIntegrationEventHandler(ISender sender)
	: IntegrationEventHandler<UserNameChangedIntegrationEvent>
{
	public override async Task Handle(
		UserNameChangedIntegrationEvent integrationEvent,
		CancellationToken cancellationToken = default) =>
		await sender
		   .Send(new ChangePlayerNameCommand(
				integrationEvent.UserId,
				integrationEvent.FirstName,
				integrationEvent.LastName), cancellationToken)
		   .EnsureSuccessAsync(error => throw new TavernTrashersException(nameof(ChangePlayerNameCommand), error));
}