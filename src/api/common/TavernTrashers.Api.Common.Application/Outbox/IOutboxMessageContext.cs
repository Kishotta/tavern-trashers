namespace TavernTrashers.Api.Common.Application.Outbox;

public interface IOutboxMessageContext
{
	string? CreatedBy { get; }
}
