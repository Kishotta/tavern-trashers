using TavernTrashers.Api.Common.Application.EventBus;
using TavernTrashers.Api.Common.Application.Exceptions;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Campaigns.Application.Members;
using TavernTrashers.Api.Modules.Users.IntegrationEvents;

namespace TavernTrashers.Api.Modules.Campaigns.Presentation.Members;

public sealed class UserNameChangedIntegrationEventHandler(ISender sender)
	: IntegrationEventHandler<UserNameChangedIntegrationEvent>
{
	public override async Task Handle(
		UserNameChangedIntegrationEvent integrationEvent,
		CancellationToken cancellationToken = default) =>
		await sender
		   .Send(new ChangeMemberNameCommand(
				integrationEvent.UserId,
				integrationEvent.FirstName,
				integrationEvent.LastName), cancellationToken)
		   .EnsureSuccessAsync(error => throw new TavernTrashersException(nameof(ChangeMemberNameCommand), error));
}