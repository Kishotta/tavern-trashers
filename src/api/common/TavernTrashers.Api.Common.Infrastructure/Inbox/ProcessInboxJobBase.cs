using System.Data;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quartz;
using TavernTrashers.Api.Common.Application.Clock;
using TavernTrashers.Api.Common.Application.Data;
using TavernTrashers.Api.Common.Application.EventBus;
using TavernTrashers.Api.Common.Infrastructure.Modules;
using TavernTrashers.Api.Common.Infrastructure.Serialization;

namespace TavernTrashers.Api.Common.Infrastructure.Inbox;

public abstract class ProcessInboxJobBase(
	IDbConnectionFactory dbConnectionFactory,
	IServiceScopeFactory serviceScopeFactory,
	IDateTimeProvider dateTimeProvider,
	ILogger logger) : IJob
{
	protected abstract IModule Module { get; }
	protected abstract int BatchSize { get; }

	public async Task Execute(IJobExecutionContext context)
	{
		logger.LogTrace("{Module} - Beginning to process inbox messages", Module.Name);

		await using var connection  = await dbConnectionFactory.OpenConnectionAsync();
		await using var transaction = await connection.BeginTransactionAsync();

		var inboxMessages = await GetUnprocessedInboxMessagesAsync(connection, transaction);

		foreach (var inboxMessage in inboxMessages)
		{
			Exception? exception = null;
			try
			{
				var integrationEvent =
					JsonConvert.DeserializeObject<IIntegrationEvent>(inboxMessage.Content,
						SerializerSettings.Instance)!;

				await PublishIntegrationEvent(integrationEvent);
			}
			catch (Exception caughtException)
			{
				logger.LogError(caughtException, "{Module} - Exception while processing inbox message {MessageId}",
					Module.Name, inboxMessage.Id);

				exception = caughtException;
			}

			await UpdateInboxMessageAsync(connection, transaction, inboxMessage, exception);
		}

		await transaction.CommitAsync();

		logger.LogDebug("{Module} - Completed processing inbox messages", Module.Name);
	}

	private async Task PublishIntegrationEvent(IIntegrationEvent integrationEvent)
	{
		using var scope = serviceScopeFactory.CreateScope();

		var integrationEventHandlers = IntegrationEventHandlersFactory.GetHandlers(
			integrationEvent.GetType(),
			scope.ServiceProvider,
			Module.PresentationAssembly);

		foreach (var integrationEventHandler in integrationEventHandlers)
			await integrationEventHandler.Handle(integrationEvent);
	}

	private async Task<IReadOnlyList<InboxMessageResponse>> GetUnprocessedInboxMessagesAsync(
		IDbConnection connection,
		IDbTransaction transaction)
	{
		var sql =
			$"""
			 SELECT
			    id AS {nameof(InboxMessageResponse.Id)},
			    content AS {nameof(InboxMessageResponse.Content)}
			 FROM {Module.Schema}.inbox_messages
			 WHERE processed_at_utc IS NULL
			 ORDER BY occurred_at_utc
			 LIMIT {BatchSize}
			 FOR UPDATE
			 """;

		var inboxMessages = await connection.QueryAsync<InboxMessageResponse>(sql, transaction: transaction);
		return inboxMessages.ToList();
	}

	private async Task UpdateInboxMessageAsync(
		IDbConnection connection,
		IDbTransaction transaction,
		InboxMessageResponse inboxMessage,
		Exception? exception)
	{
		var sql =
			$"""
			 UPDATE {Module.Schema}.inbox_messages
			 SET processed_at_utc = @ProcessedAtUtc,
			    error = @Error
			 WHERE id = @Id
			 """;

		await connection.ExecuteAsync(
			sql,
			new
			{
				inboxMessage.Id,
				ProcessedAtUtc = dateTimeProvider.UtcNow,
				Error          = exception?.Message,
			}, transaction);
	}

	private sealed record InboxMessageResponse(Guid Id, string Content);
}