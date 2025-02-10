using System.Data;
using Dapper;
using TavernTrashers.Api.Common.Application.Data;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Entities;

namespace TavernTrashers.Api.Common.Infrastructure.Outbox;

public abstract class IdempotentDomainEventHandlerBase<TDomainEvent>(
	IDomainEventHandler<TDomainEvent> decorated,
	IDbConnectionFactory dbConnectionFactory)
	: DomainEventHandler<TDomainEvent> where TDomainEvent : IDomainEvent
{
	protected abstract string Schema { get; }

	public override async Task Handle(TDomainEvent domainEvent, CancellationToken cancellationToken = default)
	{
		await using var connection = await dbConnectionFactory.OpenConnectionAsync();

		var outboxMessageConsumer = new OutboxMessageConsumer(domainEvent.Id, decorated.GetType().Name);

		if (await OutboxConsumerExistsAsync(connection, outboxMessageConsumer)) return;
        
		await decorated.Handle(domainEvent, cancellationToken);

		await InsertOutboxConsumerAsync(connection, outboxMessageConsumer);
	}

	private async Task<bool> OutboxConsumerExistsAsync(
		IDbConnection connection, 
		OutboxMessageConsumer outboxMessageConsumer)
	{
		var sql =
			$"""
			 SELECT EXISTS(
			     SELECT 1
			     FROM {Schema}.outbox_message_consumers
			     WHERE outbox_message_id = @OutboxMessageId AND 
			           name = @Name
			 )
			 """;
        
		return await connection.ExecuteScalarAsync<bool>(sql, outboxMessageConsumer);
	}

	private async Task InsertOutboxConsumerAsync(
		IDbConnection connection, 
		OutboxMessageConsumer outboxMessageConsumer)
	{
		var sql =
			$"""
			 INSERT INTO {Schema}.outbox_message_consumers(outbox_message_id, name)
			 VALUES (@OutboxMessageId, @Name)
			 """;
        
		await connection.ExecuteAsync(sql, outboxMessageConsumer);
	}
}