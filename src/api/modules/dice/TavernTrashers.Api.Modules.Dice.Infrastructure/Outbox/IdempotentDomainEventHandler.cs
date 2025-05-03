using TavernTrashers.Api.Common.Application.Data;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Entities;
using TavernTrashers.Api.Common.Infrastructure.Outbox;

namespace TavernTrashers.Api.Modules.Dice.Infrastructure.Outbox;

public class IdempotentDomainEventHandler<TDomainEvent>(
	IDomainEventHandler<TDomainEvent> decorated,
	IDbConnectionFactory dbConnectionFactory)
	: IdempotentDomainEventHandlerBase<TDomainEvent>(decorated, dbConnectionFactory)
	where TDomainEvent : IDomainEvent
{
	protected override string Schema => new DiceModule().Schema;
}