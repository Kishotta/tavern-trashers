using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using TavernTrashers.Api.Common.Application.Caching;

namespace TavernTrashers.Api.Common.Infrastructure.Caching;

public static class CacheExtensions
{
	public static void AddDistributedCache(this IHostApplicationBuilder builder)
	{
		try
		{
			builder.AddRedisDistributedCache(connectionName: "cache");
		}
		catch
		{
			// HACK: Allows application to run without a Redis server. This is useful when, for example, generating a database migration.
			builder.Services.AddDistributedMemoryCache();
		}
	}
	
	internal static IServiceCollection AddCache(this IServiceCollection services)
	{
		services.TryAddSingleton<ICacheService, CacheService>();

		return services;
	}
}