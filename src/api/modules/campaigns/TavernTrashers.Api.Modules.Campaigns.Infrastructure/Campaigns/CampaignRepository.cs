using Microsoft.EntityFrameworkCore;
using TavernTrashers.Api.Common.Application.Authentication;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns;
using TavernTrashers.Api.Modules.Campaigns.Infrastructure.Database;

namespace TavernTrashers.Api.Modules.Campaigns.Infrastructure.Campaigns;

public class CampaignRepository(CampaignsDbContext dbContext, IClaimsProvider claims) : ICampaignRepository
{
	public async Task<IEnumerable<Campaign>> GetAsync(CancellationToken cancellationToken = default) =>
		await dbContext
		   .Campaigns
		   .Where(campaign => campaign.Members.Any(member =>
				member.PlayerId == claims.UserId && member.Status == MembershipStatus.Joined))
		   .ToListAsync(cancellationToken);

	public async Task<Result<Campaign>> GetAsync(Guid campaignId, CancellationToken cancellationToken = default) =>
		await dbContext
		   .Campaigns
		   .Where(campaign => campaign.Members.Any(member =>
				member.PlayerId == claims.UserId && member.Status == MembershipStatus.Joined))
		   .SingleOrDefaultAsync(campaign => campaign.Id == campaignId, cancellationToken)
		   .ToResultAsync(CampaignErrors.NotFound(campaignId));

	public async Task<IEnumerable<Campaign>> GetReadOnlyAsync(
		CancellationToken cancellationToken = default) =>
		await dbContext
		   .Campaigns
		   .AsNoTracking()
		   .Where(campaign => campaign.Members.Any(member =>
				member.PlayerId == claims.UserId && member.Status == MembershipStatus.Joined))
		   .ToListAsync(cancellationToken);

	public async Task<Result<Campaign>> GetReadOnlyAsync(
		Guid campaignId,
		CancellationToken cancellationToken = default) =>
		await dbContext
		   .Campaigns
		   .AsNoTracking()
		   .Where(campaign => campaign.Members.Any(member =>
				member.PlayerId == claims.UserId && member.Status == MembershipStatus.Joined))
		   .SingleOrDefaultAsync(campaign => campaign.Id == campaignId, cancellationToken)
		   .ToResultAsync(CampaignErrors.NotFound(campaignId));

	public void Add(Campaign campaign) => dbContext.Campaigns.Add(campaign);
	public void Remove(Campaign campaign) => dbContext.Campaigns.Remove(campaign);
}