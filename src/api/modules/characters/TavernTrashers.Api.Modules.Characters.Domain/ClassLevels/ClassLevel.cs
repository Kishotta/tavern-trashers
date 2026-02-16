using TavernTrashers.Api.Common.Domain.Entities;
using TavernTrashers.Api.Modules.Characters.Domain.Classes;

namespace TavernTrashers.Api.Modules.Characters.Domain.ClassLevels;

public sealed class ClassLevel : Entity
{
	private ClassLevel() { }

	public Guid CharacterId { get; private set; }
	public Guid CharacterClassId { get; private set; }
	public CharacterClass CharacterClass { get; private set; } = null!;
	public int Level { get; private set; }

	internal static ClassLevel Create(Guid characterId, Guid characterClassId, int level) =>
		new()
		{
			Id = Guid.NewGuid(),
			CharacterId = characterId,
			CharacterClassId = characterClassId,
			Level = level,
		};

	internal void SetLevel(int level) => Level = level;
}
