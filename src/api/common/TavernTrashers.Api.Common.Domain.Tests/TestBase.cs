using Bogus;
using Shouldly;
using TavernTrashers.Api.Common.Domain.Entities;

namespace TavernTrashers.Api.Common.Domain.Tests;

public abstract class TestBase
{
	protected static readonly Faker Faker = new();

	public static TDomainEvent AssertDomainEventWasPublished<TDomainEvent>(EntityBase entity)
		where TDomainEvent : class, IDomainEvent
	{
		var domainEvent = entity.GetDomainEvents().OfType<TDomainEvent>().SingleOrDefault();

		domainEvent.ShouldNotBeNull($"{typeof(TDomainEvent).Name} was not published");

		return domainEvent;
	}
}