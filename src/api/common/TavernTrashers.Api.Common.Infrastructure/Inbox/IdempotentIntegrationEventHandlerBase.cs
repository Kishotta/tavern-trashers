using System.Data;
using Dapper;
using TavernTrashers.Api.Common.Application.Data;
using TavernTrashers.Api.Common.Application.EventBus;

namespace TavernTrashers.Api.Common.Infrastructure.Inbox;

public abstract class IdempotentIntegrationEventHandlerBase<TIntegrationEvent>(
	IIntegrationEventHandler<TIntegrationEvent> decorated,
	IDbConnectionFactory dbConnectionFactory)
	: IntegrationEventHandler<TIntegrationEvent>
	where TIntegrationEvent : IntegrationEvent
{
	protected abstract string Schema { get; }

	public override async Task Handle(TIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
	{
		await using var connection = await dbConnectionFactory.OpenConnectionAsync();

		var inboxMessageConsumer = new InboxMessageConsumer(integrationEvent.Id, decorated.GetType().Name);

		if (await InboxConsumerExistsAsync(connection, inboxMessageConsumer)) return;
        
		await decorated.Handle(integrationEvent, cancellationToken);

		await InsertInboxConsumerAsync(connection, inboxMessageConsumer);
	}

	private async Task<bool> InboxConsumerExistsAsync(
		IDbConnection connection, 
		InboxMessageConsumer inboxMessageConsumer)
	{
		var sql =
			$"""
			 SELECT EXISTS(
			     SELECT 1
			     FROM {Schema}.inbox_message_consumers
			     WHERE inbox_message_id = @InboxMessageId AND 
			           name = @Name
			 )
			 """;
        
		return await connection.ExecuteScalarAsync<bool>(sql, inboxMessageConsumer);
	}

	private async Task InsertInboxConsumerAsync(
		IDbConnection connection, 
		InboxMessageConsumer inboxMessageConsumer)
	{
		var sql =
			$"""
			 INSERT INTO {Schema}.inbox_message_consumers(inbox_message_id, name)
			 VALUES (@InboxMessageId, @Name)
			 """;
        
		await connection.ExecuteAsync(sql, inboxMessageConsumer);
	}
}