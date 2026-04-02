namespace TavernTrashers.Api.Modules.Characters.Application.Hubs;

public sealed record ResourceChangedNotification(
	Guid CharacterId,
	string CharacterName,
	Guid CampaignId,
	string ResourceName,
	string OldValue,
	string NewValue,
	string Actor);
