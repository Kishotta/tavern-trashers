namespace TavernTrashers.Api.Common.Infrastructure.Inbox;

public abstract class InboxOptionsBase
{
	public int IntervalInMilliseconds { get; init; }
	public int BatchSize { get; init; }
}