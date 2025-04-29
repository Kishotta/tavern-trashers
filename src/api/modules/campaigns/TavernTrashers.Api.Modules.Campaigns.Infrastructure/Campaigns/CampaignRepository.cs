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
		   .ToListAsync(cancellationToken);

	public async Task<Result<Campaign>> GetAsync(Guid campaignId, CancellationToken cancellationToken = default) =>
		await dbContext
		   .Campaigns
		   .SingleOrDefaultAsync(campaign => campaign.Id == campaignId, cancellationToken)
		   .ToResultAsync(CampaignErrors.NotFound);

	public async Task<IEnumerable<Campaign>> GetReadOnlyAsync(
		CancellationToken cancellationToken = default) =>
		await dbContext
		   .Campaigns
		   .AsNoTracking()
		   .ToListAsync(cancellationToken);

	public async Task<Result<Campaign>> GetReadOnlyAsync(
		Guid campaignId,
		CancellationToken cancellationToken = default) =>
		await dbContext
		   .Campaigns
		   .AsNoTracking()
		   .SingleOrDefaultAsync(campaign => campaign.Id == campaignId, cancellationToken)
		   .ToResultAsync(CampaignErrors.NotFound);

	public void Add(Campaign campaign) => dbContext.Campaigns.Add(campaign);
	public void Remove(Campaign campaign) => dbContext.Campaigns.Remove(campaign);

	public async Task<IEnumerable<Invitation>> GetMemberInvitationsReadOnlyAsync(
		CancellationToken cancellationToken = default) =>
		await dbContext
		   .Campaigns
		   .AsNoTracking()
		   .SelectMany(campaign => campaign.Invitations)
		   .Where(invitation => invitation.Email == claims.GetEmail())
		   .ToListAsync(cancellationToken);
}