using System.Data;
using System.Reflection;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quartz;
using TavernTrashers.Api.Common.Application.Clock;
using TavernTrashers.Api.Common.Application.Data;
using TavernTrashers.Api.Common.Domain.Entities;
using TavernTrashers.Api.Common.Infrastructure.Modules;
using TavernTrashers.Api.Common.Infrastructure.Serialization;

namespace TavernTrashers.Api.Common.Infrastructure.Outbox;

public abstract class ProcessOutboxJobBase(
    IDbConnectionFactory dbConnectionFactory,
    IServiceScopeFactory serviceScopeFactory,
    IDateTimeProvider dateTimeProvider,
    ILogger logger) : IJob
{
    protected abstract IModule Module { get; }
    protected abstract int BatchSize { get; }
    
    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogTrace("{Module} - Beginning to process outbox messages", Module.Name);
        
        await using var connection = await dbConnectionFactory.OpenConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();

        var outboxMessages = await GetUnprocessedOutboxMessagesAsync(connection, transaction);
        
        foreach (var outboxMessage in outboxMessages)
        {
            Exception? exception = null;
            try
            {
                var domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(outboxMessage.Content, SerializerSettings.Instance)!;

                await PublishDomainEvent(domainEvent);
            }
            catch (Exception caughtException)
            {
                logger.LogError(caughtException, "{Module} - Exception while processing outbox message {MessageId}", Module.Name, outboxMessage.Id);
                
                exception = caughtException;
            }
            
            await UpdateOutboxMessageAsync(connection, transaction, outboxMessage, exception);
        }
        
        await transaction.CommitAsync();
        
        logger.LogDebug("{Module} - Completed processing outbox messages", Module.Name);
    }

    private async Task PublishDomainEvent(IDomainEvent domainEvent)
    {
        using var scope = serviceScopeFactory.CreateScope();

        var domainEventHandlers = DomainEventHandlersFactory.GetHandlers(
            domainEvent.GetType(),
            scope.ServiceProvider,
            Module.ApplicationAssembly);

        foreach (var domainEventHandler in domainEventHandlers)
        {
            await domainEventHandler.Handle(domainEvent);
        }
    }

    private async Task<IReadOnlyList<OutboxMessageResponse>> GetUnprocessedOutboxMessagesAsync(
        IDbConnection connection,
        IDbTransaction transaction)
    {
        var sql =
            $"""
             SELECT
                id AS {nameof(OutboxMessageResponse.Id)},
                content AS {nameof(OutboxMessageResponse.Content)}
             FROM {Module.Schema}.outbox_messages
             WHERE processed_at_utc IS NULL
             ORDER BY occurred_at_utc
             LIMIT {BatchSize}
             FOR UPDATE
             """;

        var outboxMessages = await connection.QueryAsync<OutboxMessageResponse>(sql, transaction: transaction);
        return outboxMessages.ToList();
    }
    
    private async Task UpdateOutboxMessageAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        OutboxMessageResponse outboxMessage,
        Exception? exception)
    {
        var sql =
            $"""
             UPDATE {Module.Schema}.outbox_messages
             SET processed_at_utc = @ProcessedAtUtc,
                error = @Error
             WHERE id = @Id
             """;

        await connection.ExecuteAsync(
            sql, 
            new
            {
                outboxMessage.Id,
                ProcessedAtUtc = dateTimeProvider.UtcNow,
                Error = exception?.Message
            }, transaction: transaction);
    }

    private sealed record OutboxMessageResponse(Guid Id, string Content);
}