using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Modules.Campaigns.Application.Campaigns.GetCampaigns;
using TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns;

namespace TavernTrashers.Api.Modules.Campaigns.Application.Campaigns.GetCampaign;

internal sealed class GetCampaignQueryHandler(ICampaignRepository campaignRepository) : IQueryHandler<GetCampaignQuery, CampaignResponse>
{
	public async Task<Result<CampaignResponse>> Handle(GetCampaignQuery query, CancellationToken cancellationToken)
	{
		var campaign = await campaignRepository.GetAsync(query.CampaignId, cancellationToken);
		return campaign.IsFailure
			? campaign.Error
			: (CampaignResponse)campaign.Value;
	}
}