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

builder.Services
   .AddCors(options =>
		options.AddDefaultPolicy(configurePolicy => configurePolicy
		   .AllowAnyMethod()
		   .AllowAnyHeader()
		   .AllowAnyOrigin()));

var app = builder.Build();

app.UseCors();

app.UseAuthentication()
   .UseAuthorization();

app.MapReverseProxy();

await app.RunAsync();