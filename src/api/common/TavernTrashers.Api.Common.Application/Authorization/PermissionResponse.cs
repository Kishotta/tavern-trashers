namespace TavernTrashers.Api.Common.Application.Authorization;

public sealed record PermissionResponse(Guid UserId, HashSet<string> Permissions);