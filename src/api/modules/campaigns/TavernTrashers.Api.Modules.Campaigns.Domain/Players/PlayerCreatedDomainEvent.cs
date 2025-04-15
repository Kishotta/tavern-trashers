namespace TavernTrashers.Api.Modules.Campaigns.Domain.Players;

public sealed class PlayerCreatedDomainEvent(Guid playerId) : DomainEvent
{
	public Guid PlayerId { get; } = playerId;
}

public sealed class PlayerNameChangedDomainEvent(Guid playerId, string firstName, string lastName) : DomainEvent
{
	public Guid PlayerId { get; } = playerId;
	public string FirstName { get; } = firstName;
	public string LastName { get; } = lastName;
}