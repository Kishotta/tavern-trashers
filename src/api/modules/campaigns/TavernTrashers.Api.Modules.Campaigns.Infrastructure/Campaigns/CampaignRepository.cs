using Microsoft.EntityFrameworkCore;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns;
using TavernTrashers.Api.Modules.Campaigns.Infrastructure.Database;

namespace TavernTrashers.Api.Modules.Campaigns.Infrastructure.Campaigns;

public class CampaignRepository(CampaignsDbContext dbContext) : ICampaignRepository
{
	public async Task<IEnumerable<Campaign>> GetAsync(CancellationToken cancellationToken = default) =>
		await dbContext
		   .Campaigns
		   .Include(campaign => campaign.Questionnaires)
		   .ThenInclude(questionnaire => questionnaire.Questions)
		   .ThenInclude(question => question.Choices)
		   .ToListAsync(cancellationToken);

	public async Task<Result<Campaign>> GetAsync(Guid campaignId, CancellationToken cancellationToken = default) =>
		await dbContext
		   .Campaigns
		   .Include(campaign => campaign.Questionnaires)
		   .ThenInclude(questionnaire => questionnaire.Questions)
		   .ThenInclude(question => question.Choices)
		   .SingleOrDefaultAsync(campaign => campaign.Id == campaignId, cancellationToken: cancellationToken)
		   .ToResultAsync(CampaignErrors.NotFound(campaignId));

	public void Add(Campaign campaign) => dbContext.Campaigns.Add(campaign);
	public void Remove(Campaign campaign) => dbContext.Campaigns.Remove(campaign);
}