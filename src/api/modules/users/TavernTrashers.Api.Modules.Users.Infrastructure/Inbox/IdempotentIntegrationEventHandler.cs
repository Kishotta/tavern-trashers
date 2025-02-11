using TavernTrashers.Api.Common.Application.Data;
using TavernTrashers.Api.Common.Application.EventBus;
using TavernTrashers.Api.Common.Infrastructure.Inbox;

namespace TavernTrashers.Api.Modules.Users.Infrastructure.Inbox;

public class IdempotentIntegrationEventHandler<TIntegrationEvent>(
	IIntegrationEventHandler<TIntegrationEvent> decorated,
	IDbConnectionFactory dbConnectionFactory) 
	: IdempotentIntegrationEventHandlerBase<TIntegrationEvent>(decorated, dbConnectionFactory) 
	where TIntegrationEvent : IntegrationEvent
{
	protected override string Schema => new UsersModule().Schema;
}