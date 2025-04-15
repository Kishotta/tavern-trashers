using MediatR;
using TavernTrashers.Api.Common.Application.EventBus;
using TavernTrashers.Api.Common.Application.Exceptions;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Users.Application.Users.GetUser;
using TavernTrashers.Api.Modules.Users.Domain.Users;
using TavernTrashers.Api.Modules.Users.IntegrationEvents;

namespace TavernTrashers.Api.Modules.Users.Application.Users.RegisterUser;

internal sealed class UserRegisteredDomainEventHandler(
	ISender sender,
	IEventBus eventBus)
	: DomainEventHandler<UserRegisteredDomainEvent>
{
	public override async Task Handle(
		UserRegisteredDomainEvent domainEvent,
		CancellationToken cancellationToken = default) =>
		await sender
		   .Send(new GetUserQuery(domainEvent.UserId), cancellationToken)
		   .EnsureSuccessAsync(error => new TavernTrashersException(nameof(GetUserQuery), error))
		   .DoAsync(user => eventBus.PublishAsync(
				new UserRegisteredIntegrationEvent(
					domainEvent.Id,
					domainEvent.OccurredAtUtc,
					user.Id,
					user.Email,
					user.FirstName,
					user.LastName), cancellationToken));
}