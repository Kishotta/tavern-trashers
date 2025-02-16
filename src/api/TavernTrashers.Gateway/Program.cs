using TavernTrashers.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHttpForwarderWithServiceDiscovery();

builder.Services
   .AddReverseProxy()
   .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services
   .AddAuthentication()
   .AddKeycloakJwtBearer(
      "identity",
      "tavern-trashers",
      options =>
      {
         options.RequireHttpsMetadata = false;
         options.Audience             = "account";
      });

builder.Services.AddAuthorizationBuilder();

var app = builder.Build();

app.UseAuthentication()
   .UseAuthorization();

app.MapReverseProxy();

await app.RunAsync();