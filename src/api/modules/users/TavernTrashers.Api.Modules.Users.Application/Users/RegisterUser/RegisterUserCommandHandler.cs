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
	public async Task<Result<AuthToken>> Handle(RegisterUserCommand request, CancellationToken cancellationToken) =>
		await identityProvider
		   .RegisterUserAsync(new UserModel(
				request.Email,
				request.Password,
				request.FirstName,
				request.LastName), cancellationToken)
		   .ThenAsync(identityId => User.Create(
				request.Email,
				request.FirstName,
				request.LastName,
				identityId))
		   .DoAsync(userRepository.Insert)
		   .DoAsync(_ => unitOfWork.SaveChangesAsync(cancellationToken))
		   .ThenAsync(_ => identityProvider.GetUserAuthTokenAsync(
				request.Email, 
				request.Password, 
				cancellationToken));
}