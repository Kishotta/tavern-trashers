using MassTransit;
using TavernTrashers.Api.Common.Application.EventBus;

namespace TavernTrashers.Api.Common.Infrastructure.EventBus;

internal sealed class EventBus(IBus bus) : IEventBus
{
	public async Task PublishAsync<TIntegrationEvent>(
		TIntegrationEvent integrationEvent, 
		CancellationToken cancellationToken = default) 
		where TIntegrationEvent : IIntegrationEvent
	{
		await bus.Publish(integrationEvent, cancellationToken);
	}
}