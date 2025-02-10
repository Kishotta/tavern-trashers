namespace TavernTrashers.Api.Common.Infrastructure.Outbox;

public abstract class OutboxOptionsBase
{
	public int IntervalInMilliseconds { get; init; }
	public int BatchSize { get; init; }
}