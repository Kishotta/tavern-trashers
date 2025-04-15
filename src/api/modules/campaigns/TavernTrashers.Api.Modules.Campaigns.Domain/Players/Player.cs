using TavernTrashers.Api.Common.Domain.Auditing;

namespace TavernTrashers.Api.Modules.Campaigns.Domain.Players;

[Auditable]
public class Player : Entity
{
	private Player() { }
	public string FirstName { get; private set; } = string.Empty;
	public string LastName { get; private set; } = string.Empty;
	public string Email { get; private set; } = string.Empty;

	public static Player Create(
		Guid id,
		string firstName,
		string lastName,
		string email)
	{
		var player = new Player
		{
			Id        = id,
			FirstName = firstName,
			LastName  = lastName,
			Email     = email,
		};

		player.RaiseDomainEvent(new PlayerCreatedDomainEvent(player.Id));

		return player;
	}

	public Player ChangeName(string firstName, string lastName)
	{
		if (FirstName == firstName && LastName == lastName) return this;

		FirstName = firstName;
		LastName  = lastName;

		RaiseDomainEvent(new PlayerNameChangedDomainEvent(Id, firstName, lastName));

		return this;
	}
}