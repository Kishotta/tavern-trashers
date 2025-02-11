using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Users.Application.Abstractions.Data;
using TavernTrashers.Api.Modules.Users.Domain.Users;

namespace TavernTrashers.Api.Modules.Users.Application.Users.ChangeUserName;

internal sealed class ChangeUserNameCommandHandler(
	IUserRepository userRepository,
	IUnitOfWork unitOfWork) : ICommandHandler<ChangeUserNameCommand, UserResponse>
{
	public async Task<Result<UserResponse>> Handle(ChangeUserNameCommand request, CancellationToken cancellationToken) =>
		await userRepository
		   .GetAsync(request.UserId, cancellationToken)
		   .DoAsync(user => user.ChangeName(request.FirstName, request.LastName))
		   .DoAsync(_ => unitOfWork.SaveChangesAsync(cancellationToken))
		   .TransformAsync(user => (UserResponse)user);
}