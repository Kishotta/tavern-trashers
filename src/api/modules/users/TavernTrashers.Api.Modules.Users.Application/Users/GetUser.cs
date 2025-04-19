using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Users.Domain.Users;

namespace TavernTrashers.Api.Modules.Users.Application.Users;

public sealed record GetUserQuery(Guid UserId) : IQuery<UserResponse>;

internal sealed class GetUserQueryHandler(IUserRepository userRepository)
	: IQueryHandler<GetUserQuery, UserResponse>
{
	public async Task<Result<UserResponse>> Handle(GetUserQuery request, CancellationToken cancellationToken) =>
		await userRepository
		   .GetReadOnlyAsync(request.UserId, cancellationToken)
		   .TransformAsync(user => (UserResponse)user);
}