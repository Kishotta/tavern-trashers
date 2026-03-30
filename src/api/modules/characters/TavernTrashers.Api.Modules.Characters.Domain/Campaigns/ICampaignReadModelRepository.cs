namespace TavernTrashers.Api.Modules.Characters.Domain.Campaigns;

public interface ICampaignReadModelRepository
{
	void Add(CampaignReadModel campaign);
	Task<CampaignReadModel?> GetAsync(Guid campaignId, CancellationToken cancellationToken = default);
}
