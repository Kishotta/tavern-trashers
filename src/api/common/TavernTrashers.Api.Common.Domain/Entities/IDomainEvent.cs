using MediatR;

namespace TavernTrashers.Api.Common.Domain.Entities;

/// <summary>
/// Represents a domain event that occurred within the domain model.
/// </summary>
/// <remarks>
/// Domain events represent something significant that happened in the domain.
/// They are used to trigger side effects and communicate changes between aggregates
/// or modules. Domain events inherit from <see cref="INotification"/> to enable
/// publishing through MediatR.
/// </remarks>
public interface IDomainEvent : INotification
{
	/// <summary>
	/// Gets the unique identifier of the domain event.
	/// </summary>
	Guid Id { get; }
	
	/// <summary>
	/// Gets the UTC timestamp when the event occurred.
	/// </summary>
	DateTime OccurredAtUtc { get; }
}