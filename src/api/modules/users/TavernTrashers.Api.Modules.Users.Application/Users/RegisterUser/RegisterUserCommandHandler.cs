using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Users.Application.Abstractions.Data;
using TavernTrashers.Api.Modules.Users.Application.Abstractions.Identity;
using TavernTrashers.Api.Modules.Users.Domain.Users;

namespace TavernTrashers.Api.Modules.Users.Application.Users.RegisterUser;

internal sealed class RegisterUserCommandHandler(
	IIdentityProviderService identityProvider,
	IUserRepository userRepository,
	IUnitOfWork unitOfWork)
	: ICommandHandler<RegisterUserCommand, AuthToken>
{
	public async Task<Result<AuthToken>> Handle(
		RegisterUserCommand request,
		CancellationToken cancellationToken) =>
		await RegisterUserWithIdpAsync(request, cancellationToken)
		   .ThenAsync(identityId => CreateUserEntityAsync(identityId, request))
		   .DoAsync(userRepository.Insert)
		   .DoAsync(_ => unitOfWork.SaveChangesAsync(cancellationToken))
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