using TavernTrashers.Api.Common.Domain.Entities;

namespace TavernTrashers.Api.Modules.Characters.Domain.Characters.Events;

public sealed class CharacterDiedDomainEvent(Guid characterId) : DomainEvent
{
	public Guid CharacterId { get; } = characterId;
}
