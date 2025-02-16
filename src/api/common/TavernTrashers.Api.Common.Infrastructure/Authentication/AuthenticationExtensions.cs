using Microsoft.Extensions.DependencyInjection;

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
                    options.Audience = "account";
                });

        services.AddAuthorizationBuilder();

        services.AddHttpContextAccessor();
        
        return services;
    }
}