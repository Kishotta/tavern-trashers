using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace TavernTrashers.Web.Authentication;

public class ApiAuthenticationStateProvider : AuthenticationStateProvider
{
	private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());
	private readonly ClaimsPrincipal? _currentUser = null;
	
	public override Task<AuthenticationState> GetAuthenticationStateAsync() => 
		Task.FromResult(new AuthenticationState(_currentUser ?? _anonymous));
	public void MarkUserAsAuthenticated()
}