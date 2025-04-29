using TavernTrashers.Api.Common.Application.EventBus;
using TavernTrashers.Api.Common.Application.Exceptions;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Campaigns.Application.Members;
using TavernTrashers.Api.Modules.Users.IntegrationEvents;

namespace TavernTrashers.Api.Modules.Campaigns.Presentation.Members;

public sealed class UserRegisteredIntegrationEventHandler(ISender sender)
	: IntegrationEventHandler<UserRegisteredIntegrationEvent>
{
	public override async Task Handle(
		UserRegisteredIntegrationEvent integrationEvent,
		CancellationToken cancellationToken = default) =>
		await sender
		   .Send(new CreateMemberCommand(
				integrationEvent.UserId,
				integrationEvent.FirstName,
				integrationEvent.LastName,
				integrationEvent.Email), cancellationToken)
		   .EnsureSuccessAsync(error => throw new TavernTrashersException(nameof(CreateMemberCommand), error));
}