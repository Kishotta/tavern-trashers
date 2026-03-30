using Microsoft.EntityFrameworkCore;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns;
using TavernTrashers.Api.Modules.Campaigns.Infrastructure.Database;

namespace TavernTrashers.Api.Modules.Campaigns.Infrastructure.Campaigns;

internal sealed class CampaignRepository(CampaignsDbContext dbContext) : ICampaignRepository
{
    public void Add(Campaign campaign) => dbContext.Campaigns.Add(campaign);

    public async Task<Result<Campaign>> GetAsync(Guid campaignId, CancellationToken cancellationToken = default) =>
        await dbContext
           .Campaigns
           .SingleOrDefaultAsync(c => c.Id == campaignId, cancellationToken)
           .ToResultAsync(CampaignErrors.NotFound(campaignId));

    public async Task<IReadOnlyCollection<Campaign>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext
           .Campaigns
           .AsNoTracking()
           .ToListAsync(cancellationToken);

    public async Task<IReadOnlyCollection<Campaign>> GetByDmAsync(Guid dmUserId, CancellationToken cancellationToken = default) =>
        await dbContext
           .Campaigns
           .AsNoTracking()
           .Where(c => c.DmUserId == dmUserId)
           .ToListAsync(cancellationToken);
}
