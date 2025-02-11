using TavernTrashers.Api.Common.Application.Messaging;

namespace TavernTrashers.Api.Modules.Users.Application.Users.GetUser;

public sealed record GetUserQuery(Guid UserId) : IQuery<UserResponse>;