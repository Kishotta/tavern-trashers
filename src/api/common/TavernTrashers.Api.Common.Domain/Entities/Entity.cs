namespace TavernTrashers.Api.Common.Domain.Entities;

/// <summary>
/// Base class for domain entities with a strongly-typed identifier.
/// </summary>
/// <typeparam name="TId">The type of the entity's identifier.</typeparam>
/// <remarks>
/// Entities are objects that have a unique identity that runs through time and different states.
/// Two entities are considered equal if they have the same identity, regardless of their other properties.
/// </remarks>
public abstract class Entity<TId> : EntityBase
{
	/// <summary>
	/// Gets the unique identifier for this entity.
	/// </summary>
	public TId Id { get; protected init; } = default!;
}

/// <summary>
/// Base class for domain entities with a <see cref="Guid"/> identifier.
/// </summary>
/// <remarks>
/// This is a convenience base class for entities that use a GUID as their identifier,
/// which is the most common case in this application.
/// </remarks>
public abstract class Entity : Entity<Guid>;