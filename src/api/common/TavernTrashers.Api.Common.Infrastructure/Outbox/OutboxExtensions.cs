using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace TavernTrashers.Api.Common.Infrastructure.Outbox;

internal static class OutboxExtensions
{
	internal static IServiceCollection AddOutboxInterceptor(this IServiceCollection services)
	{
		services.TryAddSingleton<InsertOutboxMessagesInterceptor>();
		
		return services;
	}
}