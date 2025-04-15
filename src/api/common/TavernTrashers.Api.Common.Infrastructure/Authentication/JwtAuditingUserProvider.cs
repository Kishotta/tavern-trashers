using Microsoft.AspNetCore.Http;
using TavernTrashers.Api.Common.Application.Exceptions;
using TavernTrashers.Api.Common.Infrastructure.Auditing;
using TavernTrashers.Api.Common.Presentation.Authentication;

namespace TavernTrashers.Api.Common.Infrastructure.Authentication;

public class JwtAuditingUserProvider(IHttpContextAccessor httpContextAccessor) : IAuditingUserProvider
{
	private const string DefaultUser = "Unknown User";

	public string GetUserId()
	{
		try
		{
			return httpContextAccessor.HttpContext?.User.GetUserId().ToString() ?? DefaultUser;
		}
		catch (TavernTrashersException)
		{
			return DefaultUser;
		}
	}
}