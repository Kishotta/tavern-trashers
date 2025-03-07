using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace TavernTrashers.Web.Clients;

public class CustomBaseAddressAuthorizationMessageHandler : AuthorizationMessageHandler
{
	public CustomBaseAddressAuthorizationMessageHandler(IAccessTokenProvider provider, NavigationManager navigation)
		: base(provider, navigation)
	{
#pragma warning disable S1075
		ConfigureHandler(new[] { navigation.BaseUri, "http://localhost:5274" });
#pragma warning restore S1075
	}
}