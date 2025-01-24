using TavernTrashers.Api.Common.Application.Messaging;

namespace TavernTrashers.Api.Modules.Campaigns.Application.Campaigns.GetCampaign;

public sealed record GetCampaignQuery(Guid CampaignId) : IQuery<CampaignResponse>;