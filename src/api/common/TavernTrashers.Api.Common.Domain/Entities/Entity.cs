namespace TavernTrashers.Api.Common.Domain.Entities;

public abstract class Entity<TId> : EntityBase
{
	public TId Id { get; protected init; } = default!;
}

public abstract class Entity : Entity<Guid>;