using Bogus;
using TavernTrashers.Api.Common.Domain.Entities;

namespace TavernTrashers.Api.Common.Testing;

public abstract class TestBase
{
	protected static readonly Faker Faker = new();
	
	public static TDomainEvent AssertDomainEventWasPublished<TDomainEvent>(EntityBase entity)
		where TDomainEvent : IDomainEvent
	{
		var domainEvent = entity.GetDomainEvents().OfType<TDomainEvent>().SingleOrDefault();
        
		if(domainEvent is null)
			throw new Exception($"{typeof(TDomainEvent).Name} was not published");

		return domainEvent;
	}
}