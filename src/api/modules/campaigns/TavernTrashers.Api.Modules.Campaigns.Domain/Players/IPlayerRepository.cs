using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Modules.Campaigns.Domain.Players;

public interface IPlayerRepository
{
	Task<Result<Player>> GetAsync(Guid playerId, CancellationToken cancellationToken = default);
	void Add(Player player);
}