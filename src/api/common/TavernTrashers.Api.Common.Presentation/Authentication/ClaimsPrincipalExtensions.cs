using System.Security.Claims;
using TavernTrashers.Api.Common.Application.Exceptions;

namespace TavernTrashers.Api.Common.Presentation.Authentication;

public static class ClaimsPrincipalExtensions
{
	public static Guid GetUserId(this ClaimsPrincipal? principal)
	{
		var userId = principal?.FindFirstValue(CustomClaims.Sub);

		return Guid.TryParse(userId, out var parsedUserId)
			? parsedUserId
			: throw new TavernTrashersException("User identifier is unavailable");
	}

	public static string GetEmailAddress(this ClaimsPrincipal? principal) =>
		principal?.FindFirstValue(ClaimTypes.Email) ??
		throw new TavernTrashersException("User email address is unavailable");

	public static string GetIdentityId(this ClaimsPrincipal? principal) =>
		principal?.FindFirstValue(ClaimTypes.NameIdentifier) ??
		throw new TavernTrashersException("User identity is unavailable");

	public static HashSet<string> GetPermissions(this ClaimsPrincipal? principal)
	{
		var permissionClaims = principal?.FindAll(CustomClaims.Permission)
		                       ?? throw new TavernTrashersException("Permissions are unavailable");
		return permissionClaims.Select(claim => claim.Value).ToHashSet();
	}
}