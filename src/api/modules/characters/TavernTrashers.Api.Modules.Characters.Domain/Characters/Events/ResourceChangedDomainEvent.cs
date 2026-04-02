using TavernTrashers.Api.Common.Domain.Entities;

namespace TavernTrashers.Api.Modules.Characters.Domain.Characters.Events;

public sealed class ResourceChangedDomainEvent : DomainEvent
{
	public Guid CharacterId { get; }
	public string CharacterName { get; }
	public Guid CampaignId { get; }
	public string ResourceName { get; }
	public string OldValue { get; }
	public string NewValue { get; }
	public string Actor { get; }

	public ResourceChangedDomainEvent(
		Guid characterId,
		string characterName,
		Guid campaignId,
		string resourceName,
		string oldValue,
		string newValue,
		string actor)
	{
		CharacterId   = characterId;
		CharacterName = characterName;
		CampaignId    = campaignId;
		ResourceName  = resourceName;
		OldValue      = oldValue;
		NewValue      = newValue;
		Actor         = actor;
	}
}
