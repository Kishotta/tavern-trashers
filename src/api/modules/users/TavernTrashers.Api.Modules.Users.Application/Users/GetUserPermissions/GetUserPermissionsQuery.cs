using TavernTrashers.Api.Common.Application.Authorization;
using TavernTrashers.Api.Common.Application.Messaging;

namespace TavernTrashers.Api.Modules.Users.Application.Users.GetUserPermissions;

public sealed record GetUserPermissionsQuery(string IdentityId) : IQuery<PermissionResponse>;