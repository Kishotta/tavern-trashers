using FluentValidation;
using MediatR;
using TavernTrashers.Api.Common.Application.EventBus;
using TavernTrashers.Api.Common.Application.Exceptions;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Users.Application.Abstractions.Identity;
using TavernTrashers.Api.Modules.Users.Domain.Users;
using TavernTrashers.Api.Modules.Users.IntegrationEvents;

namespace TavernTrashers.Api.Modules.Users.Application.Users;

public sealed record RegisterUserCommand(
	string Email,
	string Password,
	string FirstName,
	string LastName) : ICommand<AuthToken>;

internal sealed class RegisterUserCommandHandler(
	IIdentityProviderService identityProvider,
	IUserRepository userRepository)
	: ICommandHandler<RegisterUserCommand, AuthToken>
{
	public async Task<Result<AuthToken>> Handle(
		RegisterUserCommand request,
		CancellationToken cancellationToken) =>
		await RegisterUserWithIdpAsync(request, cancellationToken)
		   .ThenAsync(identityId => CreateUserEntityAsync(identityId, request))
		   .DoAsync(userRepository.Insert)
		   .ThenAsync(user => identityProvider.GetUserAuthTokenAsync(
				user.Email,
				request.Password,
				cancellationToken));

	private Task<Result<string>> RegisterUserWithIdpAsync(
		RegisterUserCommand request,
		CancellationToken cancellationToken) =>
		identityProvider
		   .RegisterUserAsync(new(
				request.Email,
				request.Password,
				request.FirstName,
				request.LastName), cancellationToken);

	private static User CreateUserEntityAsync(
		string identityId,
		RegisterUserCommand request) =>
		User.Create(
			request.Email,
			request.FirstName,
			request.LastName,
			identityId);
}

internal sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
	public RegisterUserCommandValidator()
	{
		RuleFor(command => command.Email).NotEmpty().EmailAddress();
		RuleFor(command => command.Password).NotEmpty().MinimumLength(6);
		RuleFor(command => command.FirstName).NotEmpty();
		RuleFor(command => command.LastName).NotEmpty();
	}
}

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