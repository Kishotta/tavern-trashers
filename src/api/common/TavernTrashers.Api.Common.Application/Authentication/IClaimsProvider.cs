using System.Security.Claims;

namespace TavernTrashers.Api.Common.Application.Authentication;

public interface IClaimsProvider
{
	public ClaimsPrincipal Principal { get; }
	public Guid GetUserId();
	public string GetEmail();
}