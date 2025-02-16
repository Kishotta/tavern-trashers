using TavernTrashers.Api.Common.Domain.Auditing;

namespace TavernTrashers.Api.Modules.Campaigns.Domain.Characters;

[Auditable]
public class Character : Entity
{
	public string Name { get; private set; } = string.Empty;
	
	private Character() {}
	
	public static Character Create(string name, string playerName)
	{
		var character = new Character
		{
			Id         = Guid.NewGuid(),
			Name       = name,
		};
		
		character.RaiseDomainEvent(new CharacterCreatedDomainEvent(character.Id));
		
		return character;
	}
}