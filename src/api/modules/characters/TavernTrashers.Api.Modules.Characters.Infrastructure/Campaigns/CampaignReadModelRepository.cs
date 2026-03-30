using Microsoft.EntityFrameworkCore;
using TavernTrashers.Api.Modules.Characters.Domain.Campaigns;
using TavernTrashers.Api.Modules.Characters.Infrastructure.Database;

namespace TavernTrashers.Api.Modules.Characters.Infrastructure.Campaigns;

internal sealed class CampaignReadModelRepository(CharactersDbContext dbContext) : ICampaignReadModelRepository
{
	public void Add(CampaignReadModel campaign) => dbContext.CampaignReadModels.Add(campaign);

	public async Task<CampaignReadModel?> GetAsync(Guid campaignId, CancellationToken cancellationToken = default) =>
		await dbContext
		   .CampaignReadModels
		   .SingleOrDefaultAsync(c => c.Id == campaignId, cancellationToken);
}
