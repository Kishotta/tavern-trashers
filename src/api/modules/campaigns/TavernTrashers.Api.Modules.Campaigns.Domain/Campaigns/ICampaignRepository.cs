using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns;

public interface ICampaignRepository
{
    void Add(Campaign campaign);
    Task<Result<Campaign>> GetAsync(Guid campaignId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Campaign>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Campaign>> GetByDungeonMasterAsync(Guid dungeonMasterUserId, CancellationToken cancellationToken = default);
}
