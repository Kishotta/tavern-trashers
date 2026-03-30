using Microsoft.EntityFrameworkCore;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Characters.Domain.Campaigns;
using TavernTrashers.Api.Modules.Characters.Infrastructure.Database;

namespace TavernTrashers.Api.Modules.Characters.Infrastructure.Campaigns;

internal sealed class CampaignReadModelRepository(CharactersDbContext dbContext) : ICampaignReadModelRepository
{
	public void Add(CampaignReadModel campaign) => dbContext.CampaignReadModels.Add(campaign);

	public async Task<Result<CampaignReadModel>> GetAsync(Guid campaignId, CancellationToken cancellationToken = default) =>
		await dbContext
		   .CampaignReadModels
		   .SingleOrDefaultAsync(c => c.Id == campaignId, cancellationToken)
		   .ToResultAsync(CampaignReadModelErrors.NotFound(campaignId));
}
