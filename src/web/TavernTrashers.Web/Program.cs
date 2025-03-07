using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using TavernTrashers.Web;
using TavernTrashers.Web.Clients;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<CustomBaseAddressAuthorizationMessageHandler>();

builder.Services.AddHttpClient<ITavernTrashersClient, TavernTrashersClient>((provider, client) =>
	{
#pragma warning disable S1075
		client.BaseAddress = new Uri("http://localhost:5274");
#pragma warning restore S1075
	})
   .AddHttpMessageHandler<CustomBaseAddressAuthorizationMessageHandler>();

builder.Services.AddMudServices();

builder.Services.AddOidcAuthentication(options =>
{
	builder.Configuration.Bind("Local", options.ProviderOptions);

	options.UserOptions.NameClaim = "name";
	options.UserOptions.ScopeClaim = "scope";
});

await builder.Build().RunAsync();