using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns;

public interface ICampaignRepository
{
	Task<IEnumerable<Campaign>> GetAsync(CancellationToken cancellationToken = default);
	Task<Result<Campaign>> GetAsync(Guid campaignId, CancellationToken cancellationToken = default);

	Task<IEnumerable<Campaign>> GetReadOnlyAsync(
		CancellationToken cancellationToken = default);

	Task<Result<Campaign>> GetReadOnlyAsync(
		Guid campaignId,
		CancellationToken cancellationToken = default);

	void Add(Campaign campaign);
	void Remove(Campaign campaign);
	Task<IEnumerable<Invitation>> GetMemberInvitationsReadOnlyAsync(CancellationToken cancellationToken = default);
}