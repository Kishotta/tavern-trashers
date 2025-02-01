namespace TavernTrashers.Api.Modules.Campaigns.Domain.Characters;

public sealed class CharacterCreatedDomainEvent(Guid characterId) : DomainEvent
{
	public Guid CharacterId { get; } = characterId;
}