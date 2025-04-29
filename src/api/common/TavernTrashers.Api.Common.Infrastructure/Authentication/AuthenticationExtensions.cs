using Microsoft.Extensions.DependencyInjection;
using TavernTrashers.Api.Common.Application.Authentication;

namespace TavernTrashers.Api.Common.Infrastructure.Authentication;

internal static class AuthenticationExtensions
{
	internal static IServiceCollection AddAuthenticationInternal(this IServiceCollection services)
	{
		services.AddAuthentication()
		   .AddKeycloakJwtBearer(
				"identity",
				"tavern-trashers",
				options =>
				{
					options.RequireHttpsMetadata = false;
					options.Audience             = "account";
				});

		services.AddAuthorizationBuilder();

		services.AddHttpContextAccessor();

		services.AddSingleton<IClaimsProvider, HttpContextClaimsProvider>();

		return services;
	}
}