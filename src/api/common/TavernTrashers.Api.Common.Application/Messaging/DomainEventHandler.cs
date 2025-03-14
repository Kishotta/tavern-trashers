using TavernTrashers.Api.Common.Domain.Entities;

namespace TavernTrashers.Api.Common.Application.Messaging;

public abstract class DomainEventHandler<TDomainEvent> : IDomainEventHandler, IDomainEventHandler<TDomainEvent>
	where TDomainEvent : IDomainEvent
{
	public Task Handle(IDomainEvent domainEvent, CancellationToken cancellationToken = default) => 
		Handle((TDomainEvent)domainEvent, cancellationToken);

	public abstract Task Handle(TDomainEvent domainEvent, CancellationToken cancellationToken = default);
}