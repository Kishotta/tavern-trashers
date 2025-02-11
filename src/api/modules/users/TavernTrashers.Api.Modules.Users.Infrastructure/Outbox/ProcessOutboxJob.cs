using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TavernTrashers.Api.Common.Application.Clock;
using TavernTrashers.Api.Common.Application.Data;
using TavernTrashers.Api.Common.Infrastructure.Modules;
using TavernTrashers.Api.Common.Infrastructure.Outbox;

namespace TavernTrashers.Api.Modules.Users.Infrastructure.Outbox;

internal sealed class ProcessOutboxJob(
	IDbConnectionFactory dbConnectionFactory,
	IServiceScopeFactory serviceScopeFactory,
	IDateTimeProvider dateTimeProvider,
	IOptions<OutboxOptions> outboxOptions,
	ILogger<ProcessOutboxJob> logger)
	: ProcessOutboxJobBase(dbConnectionFactory, serviceScopeFactory, dateTimeProvider, logger)
{
	protected override IModule Module => new UsersModule();
	protected override int BatchSize => outboxOptions.Value.BatchSize;
}