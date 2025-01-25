namespace TavernTrashers.Api.Common.Domain.Entities;

public abstract class DomainEvent(Guid id, DateTime occurredAtUtc) : IDomainEvent
{
	public Guid Id { get; init; } = id;
	public DateTime OccurredAtUtc { get; init; } = occurredAtUtc;

	protected DomainEvent() : this(Guid.NewGuid(), DateTime.UtcNow) { }
}