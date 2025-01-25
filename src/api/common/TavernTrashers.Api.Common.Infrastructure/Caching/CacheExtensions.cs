using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StackExchange.Redis;
using TavernTrashers.Api.Common.Application.Caching;

namespace TavernTrashers.Api.Common.Infrastructure.Caching;

internal static class CacheExtensions
{
	internal static IServiceCollection AddCache(this IServiceCollection services)
	{
		services.TryAddSingleton<ICacheService, CacheService>();

		return services;
	}
}