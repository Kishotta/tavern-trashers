using MediatR;

namespace TavernTrashers.Api.Common.Domain.Entities;

public interface IDomainEvent : INotification
{
	Guid Id { get; }
	DateTime OccurredAtUtc { get; }
}