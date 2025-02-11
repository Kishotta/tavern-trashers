using Microsoft.Extensions.Options;
using TavernTrashers.Api.Common.Infrastructure.Outbox;

namespace TavernTrashers.Api.Modules.Users.Infrastructure.Outbox;

internal sealed class ConfigureProcessOutboxJob(IOptions<OutboxOptions> outboxOptions)
	: ConfigureProcessOutboxJobBase<OutboxOptions, ProcessOutboxJob>(outboxOptions);