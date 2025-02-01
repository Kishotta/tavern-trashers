using TavernTrashers.Api.Common.Domain.Auditing;
using TavernTrashers.Api.Modules.Campaigns.Domain.Questionnaires;

namespace TavernTrashers.Api.Modules.Campaigns.Domain.Characters;

[Auditable]
public class Character : Entity
{
	public string Name { get; private set; } = string.Empty;
	public string PlayerName { get; private set; } = string.Empty;
	
	public IReadOnlyCollection<Answer> Answers => answers.AsReadOnly();
	private readonly List<Answer> answers = [];
	
	private Character() {}
	
	public static Character Create(string name, string playerName)
	{
		var character = new Character
		{
			Id         = Guid.NewGuid(),
			Name       = name,
			PlayerName = playerName
		};
		
		character.RaiseDomainEvent(new CharacterCreatedDomainEvent(character.Id));
		
		return character;
	}
}