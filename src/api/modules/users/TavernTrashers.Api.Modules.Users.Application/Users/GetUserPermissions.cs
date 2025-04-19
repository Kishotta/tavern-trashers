using Dapper;
using TavernTrashers.Api.Common.Application.Authorization;
using TavernTrashers.Api.Common.Application.Data;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Users.Domain.Users;

namespace TavernTrashers.Api.Modules.Users.Application.Users;

public sealed record GetUserPermissionsQuery(string IdentityId) : IQuery<PermissionResponse>;

internal sealed class GetUserPermissionsQueryHandler(IDbConnectionFactory dbConnectionFactory)
	: IQueryHandler<GetUserPermissionsQuery, PermissionResponse>
{
	public async Task<Result<PermissionResponse>> Handle(
		GetUserPermissionsQuery request,
		CancellationToken cancellationToken)
	{
		await using var connection = await dbConnectionFactory.OpenConnectionAsync();

		const string sql =
			$"""
			 SELECT DISTINCT
			    u.id AS {nameof(UserPermission.UserId)},
			    rp.permission_code AS {nameof(UserPermission.Permission)}
			 FROM users.users u
			 JOIN users.user_roles ur ON u.id = ur.user_id
			 JOIN users.role_permissions rp ON ur.role_name = rp.role_name
			 WHERE u.identity_id = @IdentityId
			 """;

		return await connection
		   .QueryAsync<UserPermission>(sql, request)
		   .TransformAsync(permissions => permissions.ToList())
		   .EnsureAsync(permissions => permissions.Count != 0, UserErrors.NotFound(request.IdentityId))
		   .TransformAsync(permissions =>
				new PermissionResponse(
					permissions[0].UserId,
					permissions
					   .Select(userPermission => userPermission.Permission)
					   .ToHashSet()));
	}

	internal sealed class UserPermission
	{
		internal Guid UserId { get; init; }
		internal string Permission { get; init; } = string.Empty;
	}
}