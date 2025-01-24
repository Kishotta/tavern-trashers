using TavernTrashers.Api.Common.Application.Messaging;

namespace TavernTrashers.Api.Modules.Campaigns.Application.Campaigns.GetCampaigns;

public sealed record GetCampaignsQuery : IQuery<IReadOnlyCollection<CampaignResponse>>;