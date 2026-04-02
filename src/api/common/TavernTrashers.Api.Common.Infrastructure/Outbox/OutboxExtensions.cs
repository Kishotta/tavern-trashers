using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TavernTrashers.Api.Common.Application.Outbox;

namespace TavernTrashers.Api.Common.Infrastructure.Outbox;

internal static class OutboxExtensions
{
	internal static IServiceCollection AddOutboxInterceptor(this IServiceCollection services)
	{
		services.TryAddSingleton<InsertOutboxMessagesInterceptor>();

		services.TryAddScoped<OutboxMessageContext>();
		services.TryAddScoped<IOutboxMessageContext>(sp => sp.GetRequiredService<OutboxMessageContext>());

		return services;
	}
}