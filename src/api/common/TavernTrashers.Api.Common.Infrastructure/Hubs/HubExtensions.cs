using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using TavernTrashers.Api.Common.Application.Hubs;

namespace TavernTrashers.Api.Common.Infrastructure.Hubs;

public static class HubExtensions
{
	public static IServiceCollection AddHubs(this IServiceCollection services)
	{
		services.AddSignalR();
		services.AddSingleton<IHubService, HubService>();

		services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
		{
			var existingOnMessageReceived = options.Events?.OnMessageReceived;
			options.Events ??= new JwtBearerEvents();
			options.Events.OnMessageReceived = async context =>
			{
				if (existingOnMessageReceived is not null)
					await existingOnMessageReceived(context);

				var accessToken = context.Request.Query["access_token"];
				if (!string.IsNullOrEmpty(accessToken) &&
				    context.HttpContext.Request.Path.StartsWithSegments("/hubs"))
				{
					context.Token = accessToken;
				}
			};
		});

		return services;
	}

	public static IEndpointRouteBuilder MapTavernTrashersHub(this IEndpointRouteBuilder app)
	{
		app.MapHub<TavernTrashersHub>("/hubs/tavern-trashers");
		return app;
	}
}
