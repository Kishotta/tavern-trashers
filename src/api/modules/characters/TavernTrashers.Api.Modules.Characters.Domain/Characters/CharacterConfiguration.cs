using TavernTrashers.Api.Common.Domain.Auditing;
using TavernTrashers.Api.Common.Domain.Entities;

namespace TavernTrashers.Api.Modules.Characters.Domain.Characters;

[Auditable]
public class CharacterConfiguration : Entity
{
	public string Name { get; private set; } = string.Empty;

	public static CharacterConfiguration Create(string name)
	{
		var characterConfiguration = new CharacterConfiguration
		{
			Name = name,
		};

		return characterConfiguration;
	}
}