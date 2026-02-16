namespace TavernTrashers.Api.Common.Domain.Entities;

/// <summary>
/// Base class for all domain entities providing domain event support.
/// </summary>
/// <remarks>
/// This abstract class provides the infrastructure for raising and managing domain events.
/// Domain events are used to communicate significant changes within the domain model.
/// </remarks>
public abstract class EntityBase
{
	/// <summary>
	/// Gets all domain events that have been raised by this entity.
	/// </summary>
	/// <returns>A read-only collection of domain events.</returns>
	public IReadOnlyCollection<IDomainEvent> GetDomainEvents() => _domainEvents.ToList();
	private readonly List<IDomainEvent> _domainEvents = [];

	/// <summary>
	/// Clears all domain events from this entity.
	/// </summary>
	/// <remarks>
	/// This method should be called after domain events have been processed and published
	/// to prevent events from being published multiple times.
	/// </remarks>
	public void ClearDomainEvents()
	{
		_domainEvents.Clear();
	}
	
	/// <summary>
	/// Raises a domain event by adding it to the entity's domain event collection.
	/// </summary>
	/// <param name="domainEvent">The domain event to raise.</param>
	/// <remarks>
	/// Domain events are collected but not immediately published. They will be published
	/// after the entity changes are successfully persisted to ensure transactional consistency.
	/// </remarks>
	protected void RaiseDomainEvent(IDomainEvent domainEvent)
	{
		_domainEvents.Add(domainEvent);
	}
}