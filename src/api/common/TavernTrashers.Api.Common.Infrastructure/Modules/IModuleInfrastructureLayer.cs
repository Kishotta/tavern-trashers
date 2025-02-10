using MassTransit;

namespace TavernTrashers.Api.Common.Infrastructure.Modules;

public interface IModuleInfrastructureLayer
{
	Type IdempotentDomainEventHandlerType { get; }
	Type IdempotentIntegrationEventHandlerType { get; }
	Action<IRegistrationConfigurator> ConfigureConsumers { get; }
}