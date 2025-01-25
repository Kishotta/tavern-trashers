using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Common.Application.Authorization;

public interface IPermissionService
{
    Task<Result<PermissionResponse>> GetUserPermissionsAsync(string identityId);
}