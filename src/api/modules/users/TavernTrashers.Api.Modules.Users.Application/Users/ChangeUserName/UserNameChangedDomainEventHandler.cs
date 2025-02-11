using TavernTrashers.Api.Common.Application.EventBus;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Modules.Users.Domain.Users;
using TavernTrashers.Api.Modules.Users.IntegrationEvents;

namespace TavernTrashers.Api.Modules.Users.Application.Users.ChangeUserName;

internal sealed class UserNameChangedDomainEventHandler (
	IEventBus eventBus)
	: DomainEventHandler<UserNameChangedDomainEvent>
{
	public override async Task Handle(UserNameChangedDomainEvent notification, CancellationToken cancellationToken = default) =>
		await eventBus.PublishAsync(
			new UserNameChangedIntegrationEvent(
				notification.Id,
				notification.OccurredAtUtc,
				notification.UserId,
				notification.FirstName,
				notification.LastName),
			cancellationToken);
}