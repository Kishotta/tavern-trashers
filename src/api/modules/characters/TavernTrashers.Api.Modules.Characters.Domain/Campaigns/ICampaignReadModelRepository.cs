using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Modules.Characters.Domain.Campaigns;

public interface ICampaignReadModelRepository
{
	void Add(CampaignReadModel campaign);
	Task<Result<CampaignReadModel>> GetAsync(Guid campaignId, CancellationToken cancellationToken = default);
}
