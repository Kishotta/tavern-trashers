using TavernTrashers.Api.Common.Domain.Entities;

namespace TavernTrashers.Api.Common.Application.Messaging;

public interface IDomainEventHandler
{
	Task Handle(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
}

public interface IDomainEventHandler<in TDomainEvent>
	where TDomainEvent : IDomainEvent
{
	Task Handle(TDomainEvent domainEvent, CancellationToken cancellationToken = default);
}