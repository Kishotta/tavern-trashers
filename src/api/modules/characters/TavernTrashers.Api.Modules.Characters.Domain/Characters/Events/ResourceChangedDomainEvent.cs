using TavernTrashers.Api.Common.Domain.Entities;

namespace TavernTrashers.Api.Modules.Characters.Domain.Characters.Events;

public sealed class ResourceChangedDomainEvent(
	Guid characterId,
	string characterName,
	Guid campaignId,
	string resourceName,
	string oldValue,
	string newValue,
	string actor) : DomainEvent
{
	public Guid CharacterId { get; } = characterId;
	public string CharacterName { get; } = characterName;
	public Guid CampaignId { get; } = campaignId;
	public string ResourceName { get; } = resourceName;
	public string OldValue { get; } = oldValue;
	public string NewValue { get; } = newValue;
	public string Actor { get; } = actor;
}
