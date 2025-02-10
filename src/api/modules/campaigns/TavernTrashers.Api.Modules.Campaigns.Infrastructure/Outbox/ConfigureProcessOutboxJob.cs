using Microsoft.Extensions.Options;
using TavernTrashers.Api.Common.Infrastructure.Outbox;

namespace TavernTrashers.Api.Modules.Campaigns.Infrastructure.Outbox;

internal sealed class ConfigureProcessOutboxJob(IOptions<OutboxOptions> outboxOptions)
	: ConfigureProcessOutboxJobBase<OutboxOptions, ProcessOutboxJob>(outboxOptions);