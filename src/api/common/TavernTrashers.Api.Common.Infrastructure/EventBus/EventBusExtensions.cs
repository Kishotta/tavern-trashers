using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using TavernTrashers.Api.Common.Application.EventBus;
using TavernTrashers.Api.Common.Infrastructure.Modules;

namespace TavernTrashers.Api.Common.Infrastructure.EventBus;

internal static class EventBusExtensions
{
	internal static IServiceCollection AddEventBus(
		this IServiceCollection services,
		string connectionString,
		IEnumerable<IModuleInfrastructureLayer> modules)
	{
		services.AddMassTransit(busConfigurator =>
		{
			foreach (var module in modules)
				module.ConfigureConsumers(busConfigurator);
			
			busConfigurator.SetKebabCaseEndpointNameFormatter();
			busConfigurator.UsingRabbitMq((context, config) =>
			{
				config.Host(new Uri(connectionString));
				config.ConfigureEndpoints(context);
			});
		});

		services.AddSingleton<IEventBus, EventBus>();

		return services;
	}
}