using System.Security.Claims;

namespace TavernTrashers.Api.Common.Application.Authentication;

public interface IClaimsProvider
{
	public Guid UserId { get; }
	public ClaimsPrincipal Principal { get; }
}