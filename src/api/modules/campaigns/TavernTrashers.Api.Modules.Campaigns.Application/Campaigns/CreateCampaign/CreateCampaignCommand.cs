using TavernTrashers.Api.Common.Application.Messaging;

namespace TavernTrashers.Api.Modules.Campaigns.Application.Campaigns.CreateCampaign;

public sealed record CreateCampaignCommand(Guid DungeonMasterId, string Name, string Description)
	: ICommand<CampaignResponse>;