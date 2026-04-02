using TavernTrashers.Api.Common.Application.Outbox;

namespace TavernTrashers.Api.Common.Infrastructure.Outbox;

internal sealed class OutboxMessageContext : IOutboxMessageContext
{
	public string? CreatedBy { get; set; }
}
