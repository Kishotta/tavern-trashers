using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TavernTrashers.Api.Common.Application.Clock;
using TavernTrashers.Api.Common.Application.Data;
using TavernTrashers.Api.Common.Infrastructure.Inbox;
using TavernTrashers.Api.Common.Infrastructure.Modules;

namespace TavernTrashers.Api.Modules.Users.Infrastructure.Inbox;

internal sealed class ProcessInboxJob(
	IDbConnectionFactory dbConnectionFactory,
	IServiceScopeFactory serviceScopeFactory,
	IDateTimeProvider dateTimeProvider,
	IOptions<InboxOptions> inboxOptions,
	ILogger<ProcessInboxJob> logger) 
	: ProcessInboxJobBase(dbConnectionFactory, serviceScopeFactory, dateTimeProvider, logger)
{
	protected override IModule Module => new UsersModule();
	protected override int BatchSize => inboxOptions.Value.BatchSize;
}