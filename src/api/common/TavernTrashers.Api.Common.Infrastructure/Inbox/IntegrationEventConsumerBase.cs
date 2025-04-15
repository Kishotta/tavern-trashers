using Dapper;
using MassTransit;
using Newtonsoft.Json;
using TavernTrashers.Api.Common.Application.Data;
using TavernTrashers.Api.Common.Application.EventBus;
using TavernTrashers.Api.Common.Infrastructure.Serialization;

namespace TavernTrashers.Api.Common.Infrastructure.Inbox;

public abstract class IntegrationEventConsumerBase<TIntegrationEvent>(IDbConnectionFactory dbConnectionFactory)
	: IConsumer<TIntegrationEvent> where TIntegrationEvent : IntegrationEvent
{
	protected abstract string Schema { get; }

	public async Task Consume(ConsumeContext<TIntegrationEvent> context)
	{
		await using var connection = await dbConnectionFactory.OpenConnectionAsync();

		var integrationEvent = context.Message;

		var inboxMessage = new InboxMessage
		{
			Id            = integrationEvent.Id,
			Type          = typeof(TIntegrationEvent).Name,
			Content       = JsonConvert.SerializeObject(integrationEvent, SerializerSettings.Instance),
			OccurredAtUtc = integrationEvent.OccurredAtUtc,
		};

		var sql =
			$"""
			 INSERT INTO {Schema}.inbox_messages (id, type, content, occurred_at_utc)
			 VALUES (@Id, @Type, @Content::json, @OccurredAtUtc)
			 """;

		await connection.ExecuteAsync(sql, inboxMessage);
	}
}