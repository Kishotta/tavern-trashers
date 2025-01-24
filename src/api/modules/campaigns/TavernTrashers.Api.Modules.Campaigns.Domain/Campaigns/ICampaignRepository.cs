using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns;

public interface ICampaignRepository
{
	Task<IEnumerable<Campaign>> GetAsync(CancellationToken cancellationToken = default);
	Task<Result<Campaign>> GetAsync(CampaignId campaignId, CancellationToken cancellationToken = default);
	void Add(Campaign campaign);
	void Remove(Campaign campaign);
}