using Microsoft.EntityFrameworkCore;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Campaigns.Domain.Players;
using TavernTrashers.Api.Modules.Campaigns.Infrastructure.Database;

namespace TavernTrashers.Api.Modules.Campaigns.Infrastructure.Players;

public class PlayerRepository(CampaignsDbContext dbContext) : IPlayerRepository
{
	public async Task<Result<Player>> GetAsync(Guid playerId, CancellationToken cancellationToken = default) =>
		await dbContext
		   .Players
		   .SingleOrDefaultAsync(player => player.Id == playerId, cancellationToken)
		   .ToResultAsync(PlayerErrors.NotFound(playerId));

	public void Add(Player player) => dbContext.Players.Add(player);
}