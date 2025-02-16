using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using TavernTrashers.Web;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddMudServices();

builder.Services.AddOidcAuthentication(options =>
{
	builder.Configuration.Bind("Local", options.ProviderOptions);

	options.UserOptions.NameClaim = "name";
	options.UserOptions.ScopeClaim = "scope";
});

await builder.Build().RunAsync();