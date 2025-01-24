using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns;

namespace TavernTrashers.Api.Modules.Campaigns.Application.Campaigns.GetCampaigns;

internal sealed class GetCampaignsQueryHandler(ICampaignRepository campaignRepository) : IQueryHandler<GetCampaignsQuery, IReadOnlyCollection<CampaignResponse>>
{
	public async Task<Result<IReadOnlyCollection<CampaignResponse>>> Handle(GetCampaignsQuery query, CancellationToken cancellationToken)
	{
		var campaigns = await campaignRepository.GetAsync(cancellationToken);

		return campaigns
		   .Select(campaign => (CampaignResponse)campaign)
		   .ToList()
		   .AsReadOnly();
	}
}