using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using TavernTrashers.Api.Common.Application.Authentication;
using TavernTrashers.Api.Common.Application.Exceptions;
using TavernTrashers.Api.Common.Presentation.Authentication;

namespace TavernTrashers.Api.Common.Infrastructure.Authentication;

public class HttpContextClaimsProvider(IHttpContextAccessor httpContextAccessor) : IClaimsProvider
{
	public Guid GetUserId() => Principal.GetUserId();
	public string GetEmail() => Principal.GetEmailAddress();

	public ClaimsPrincipal Principal =>
		httpContextAccessor.HttpContext?.User ??
		throw new TavernTrashersException(
			"User claims principal is unavailable");
}