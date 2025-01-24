using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TavernTrashers.Api.Common.Infrastructure.Outbox;

namespace TavernTrashers.Api.Common.Infrastructure;

public static class InfrastructureLayerExtensions
{
	public static IServiceCollection ConfigureInfrastructureLayer(this IServiceCollection services)
	{
		services.TryAddSingleton<InsertOutboxMessagesInterceptor>();

		return services;
	}
}