using Shouldly;
using TavernTrashers.Api.Common.Domain.Entities;

namespace TavernTrashers.Api.Common.Domain.Tests;

public class EntityBaseTests : TestBase
{
	private class TestEntity : EntityBase
	{
		public void RaiseTestEvent(IDomainEvent domainEvent)
		{
			RaiseDomainEvent(domainEvent);
		}
	}

	private class TestDomainEvent : DomainEvent
	{
		public string Message { get; }

		public TestDomainEvent(string message) : base()
		{
			Message = message;
		}
	}

	[Fact]
	public void GetDomainEvents_Should_Return_Empty_Collection_Initially()
	{
		// Arrange
		var entity = new TestEntity();

		// Act
		var events = entity.GetDomainEvents();

		// Assert
		events.ShouldBeEmpty();
	}

	[Fact]
	public void RaiseDomainEvent_Should_Add_Event_To_Collection()
	{
		// Arrange
		var entity = new TestEntity();
		var domainEvent = new TestDomainEvent("Test message");

		// Act
		entity.RaiseTestEvent(domainEvent);
		var events = entity.GetDomainEvents();

		// Assert
		events.ShouldNotBeEmpty();
		events.Count.ShouldBe(1);
		events.First().ShouldBe(domainEvent);
	}

	[Fact]
	public void RaiseDomainEvent_Multiple_Times_Should_Add_All_Events()
	{
		// Arrange
		var entity = new TestEntity();
		var event1 = new TestDomainEvent("Message 1");
		var event2 = new TestDomainEvent("Message 2");
		var event3 = new TestDomainEvent("Message 3");

		// Act
		entity.RaiseTestEvent(event1);
		entity.RaiseTestEvent(event2);
		entity.RaiseTestEvent(event3);
		var events = entity.GetDomainEvents();

		// Assert
		events.Count.ShouldBe(3);
		events.ShouldContain(event1);
		events.ShouldContain(event2);
		events.ShouldContain(event3);
	}

	[Fact]
	public void ClearDomainEvents_Should_Remove_All_Events()
	{
		// Arrange
		var entity = new TestEntity();
		var domainEvent = new TestDomainEvent("Test message");
		entity.RaiseTestEvent(domainEvent);

		// Act
		entity.ClearDomainEvents();
		var events = entity.GetDomainEvents();

		// Assert
		events.ShouldBeEmpty();
	}

	[Fact]
	public void GetDomainEvents_Should_Return_ReadOnly_Collection()
	{
		// Arrange
		var entity = new TestEntity();
		var domainEvent = new TestDomainEvent("Test message");
		entity.RaiseTestEvent(domainEvent);

		// Act
		var events = entity.GetDomainEvents();

		// Assert
		events.ShouldBeOfType<List<IDomainEvent>>();
		events.ShouldBeAssignableTo<IReadOnlyCollection<IDomainEvent>>();
	}
}
