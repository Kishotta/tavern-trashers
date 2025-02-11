using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Quartz;
using TavernTrashers.Api.Common.Application.EventBus;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Infrastructure.Auditing;
using TavernTrashers.Api.Common.Infrastructure.Authentication;
using TavernTrashers.Api.Common.Infrastructure.Authorization;
using TavernTrashers.Api.Common.Infrastructure.Caching;
using TavernTrashers.Api.Common.Infrastructure.Clock;
using TavernTrashers.Api.Common.Infrastructure.Database;
using TavernTrashers.Api.Common.Infrastructure.EventBus;
using TavernTrashers.Api.Common.Infrastructure.Inbox;
using TavernTrashers.Api.Common.Infrastructure.Modules;
using TavernTrashers.Api.Common.Infrastructure.Outbox;

namespace TavernTrashers.Api.Common.Infrastructure;

public static class InfrastructureLayerExtensions
{
	public static IServiceCollection ConfigureInfrastructureLayer(
		this IServiceCollection services,
		IConfiguration configuration,
		IEnumerable<IModule> modules)
	{
		services
		   .AddRouting()
		   .AddAuthorizationInternal()
		   .AddAuthenticationInternal()
		   .AddAuditing()
		   .AddCache()
		   .AddDateTimeProvider()
		   .AddDbConnectionFactory(configuration.GetConnectionString("database")!)
		   .AddEventBus(configuration.GetConnectionString("queue")!, modules)
		   .AddOutboxInterceptor()
		   .AddQuartzJobs();

		return services;
	}

	internal static IServiceCollection AddDomainEventHandlers(
		this IServiceCollection services,
		IModule module)
	{
		module.ApplicationAssembly
		   .GetTypes()
		   .Where(type => type.IsAssignableTo(typeof(IDomainEventHandler)))
		   .ToList()
		   .ForEach(domainEventHandlerType =>
			{
				services.TryAddScoped(domainEventHandlerType);

				var domainEventType = domainEventHandlerType
				   .GetInterfaces()
				   .Single(@interface => @interface.IsGenericType)
				   .GetGenericArguments()
				   .Single();

				var closedIdempotentHandlerType =
					module.IdempotentDomainEventHandlerType.MakeGenericType(domainEventType);

				services.Decorate(domainEventHandlerType, closedIdempotentHandlerType);
			});

		return services;
	}

	internal static IServiceCollection AddIntegrationEventHandlers(
		this IServiceCollection services,
		IModule module)
	{
		module.PresentationAssembly
		   .GetTypes()
		   .Where(type => type.IsAssignableTo(typeof(IIntegrationEventHandler)))
		   .ToList()
		   .ForEach(integrationEventHandlerType =>
			{
				services.TryAddScoped(integrationEventHandlerType);

				var integrationEventType = integrationEventHandlerType
				   .GetInterfaces()
				   .Single(@interface => @interface.IsGenericType)
				   .GetGenericArguments()
				   .Single();

				var closedIdempotentHandlerType =
					module.IdempotentIntegrationEventHandlerType.MakeGenericType(integrationEventType);

				services.Decorate(integrationEventHandlerType, closedIdempotentHandlerType);
			});

		return services;
	}

	private static IServiceCollection AddQuartzJobs(this IServiceCollection services)
	{
		services.AddQuartz(configurator =>
		{
			var scheduler = Guid.NewGuid();
			configurator.SchedulerId   = $"default-id-{scheduler}";
			configurator.SchedulerName = $"default-name-{scheduler}";
		});
        
		services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

		return services;
	}
}