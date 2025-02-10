using System.Reflection;
using MassTransit;
using Microsoft.Extensions.Hosting;
using TavernTrashers.Api.Common.Application.EventBus;
using TavernTrashers.Api.Common.Infrastructure.Outbox;
using TavernTrashers.Api.Common.Presentation.Endpoints;

namespace TavernTrashers.Api.Common.Infrastructure.Modules;

public abstract class Module : IModule
{
	public abstract string Name { get; }
	public abstract string Schema { get; }
	public abstract Assembly ApplicationAssembly { get; }
	public abstract Assembly PresentationAssembly { get; }
	public abstract Type IdempotentDomainEventHandlerType { get; }
	public abstract Type IdempotentIntegrationEventHandlerType { get; }
	protected abstract void AddDatabase(IHostApplicationBuilder builder);
	protected abstract void ConfigureServices(IHostApplicationBuilder builder);

	public void AddModule(IHostApplicationBuilder builder)
	{
		builder.Services
		   .AddDomainEventHandlers(this)
		   .AddIntegrationEventHandlers(this)
		   .AddEndpoints(this);
		
		AddDatabase(builder);
		ConfigureServices(builder);
	}
	
	public Action<IRegistrationConfigurator> ConfigureConsumers =>
		registrationConfigurator =>
		{
			PresentationAssembly
			   .GetTypes()
			   .Where(type => type.IsAssignableTo(typeof(IIntegrationEventHandler)))
			   .ToList()
			   .ForEach(integrationEventHandlerType =>
				{
					// IntegrationEventHandlers are generic types that must only have a single generic argument
					var integrationEventType = integrationEventHandlerType
					   .GetInterfaces()
					   .Single(@interface => @interface.IsGenericType)
					   .GetGenericArguments()
					   .Single();

					registrationConfigurator
					   .AddConsumer(typeof(IntegrationEventHandler<>).MakeGenericType(integrationEventType));
				});
		};
}