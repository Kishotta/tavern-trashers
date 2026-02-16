namespace TavernTrashers.Api.Common.Domain.Entities;

/// <summary>
/// Base implementation of <see cref="IDomainEvent"/> providing common event properties.
/// </summary>
/// <param name="id">The unique identifier for the event.</param>
/// <param name="occurredAtUtc">The UTC timestamp when the event occurred.</param>
/// <remarks>
/// Domain events should use past tense naming (e.g., CharacterCreated, DiceRolled)
/// to indicate that something has already happened in the domain.
/// </remarks>
public abstract class DomainEvent(Guid id, DateTime occurredAtUtc) : IDomainEvent
{
	/// <summary>
	/// Gets the unique identifier of the domain event.
	/// </summary>
	public Guid Id { get; init; } = id;
	
	/// <summary>
	/// Gets the UTC timestamp when the event occurred.
	/// </summary>
	public DateTime OccurredAtUtc { get; init; } = occurredAtUtc;

	/// <summary>
	/// Initializes a new instance of the <see cref="DomainEvent"/> class with auto-generated values.
	/// </summary>
	/// <remarks>
	/// This parameterless constructor automatically generates a new GUID and sets the occurrence
	/// time to the current UTC time. This is the recommended way to create domain events.
	/// </remarks>
	protected DomainEvent() : this(Guid.NewGuid(), DateTime.UtcNow) { }
}