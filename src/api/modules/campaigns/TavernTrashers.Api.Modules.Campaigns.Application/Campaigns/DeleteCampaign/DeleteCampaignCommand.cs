using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns;

namespace TavernTrashers.Api.Modules.Campaigns.Application.Campaigns.DeleteCampaign;

public sealed record DeleteCampaignCommand(Guid CampaignId) : ICommand<CampaignResponse>;