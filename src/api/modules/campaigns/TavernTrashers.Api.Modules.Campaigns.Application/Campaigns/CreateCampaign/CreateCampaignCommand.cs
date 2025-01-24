using TavernTrashers.Api.Common.Application.Messaging;

namespace TavernTrashers.Api.Modules.Campaigns.Application.Campaigns.CreateCampaign;

public sealed record CreateCampaignCommand(string Name, string Description) : ICommand<CampaignResponse>;