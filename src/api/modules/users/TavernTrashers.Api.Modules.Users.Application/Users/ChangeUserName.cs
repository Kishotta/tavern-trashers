using FluentValidation;
using TavernTrashers.Api.Common.Application.EventBus;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Users.Application.Abstractions.Data;
using TavernTrashers.Api.Modules.Users.Domain.Users;
using TavernTrashers.Api.Modules.Users.IntegrationEvents;

namespace TavernTrashers.Api.Modules.Users.Application.Users;

public sealed record ChangeUserNameCommand(
	Guid UserId,
	string FirstName,
	string LastName) : ICommand<UserResponse>;

internal sealed class ChangeUserNameCommandHandler(
	IUserRepository userRepository,
	IUnitOfWork unitOfWork) : ICommandHandler<ChangeUserNameCommand, UserResponse>
{
	public async Task<Result<UserResponse>> Handle(
		ChangeUserNameCommand request,
		CancellationToken cancellationToken) =>
		await userRepository
		   .GetAsync(request.UserId, cancellationToken)
		   .DoAsync(async user =>
			{
				user.ChangeName(request.FirstName, request.LastName);
				await unitOfWork.SaveChangesAsync(cancellationToken);
			})
		   .TransformAsync(user => (UserResponse)user);
}

internal sealed class ChangeUserNameCommandValidator : AbstractValidator<ChangeUserNameCommand>
{
	public ChangeUserNameCommandValidator()
	{
		RuleFor(command => command.UserId).NotEmpty();
		RuleFor(command => command.FirstName).NotEmpty();
		RuleFor(command => command.LastName).NotEmpty();
	}
}

internal sealed class UserNameChangedDomainEventHandler(IEventBus eventBus)
	: DomainEventHandler<UserNameChangedDomainEvent>
{
	public override async Task Handle(
		UserNameChangedDomainEvent notification,
		CancellationToken cancellationToken = default) =>
		await eventBus.PublishAsync(
			new UserNameChangedIntegrationEvent(
				notification.Id,
				notification.OccurredAtUtc,
				notification.UserId,
				notification.FirstName,
				notification.LastName),
			cancellationToken);
}