using MediatR;
using TavernTrashers.Api.Common.Application.Authorization;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Modules.Users.Application.Users.GetUserPermissions;

namespace TavernTrashers.Api.Modules.Users.Infrastructure.Authorization;

internal sealed class PermissionService(ISender sender) 
    : IPermissionService
{
    public async Task<Result<PermissionResponse>> GetUserPermissionsAsync(string identityId) => 
        await sender.Send(new GetUserPermissionsQuery(identityId));
}