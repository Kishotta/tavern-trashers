using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Modules.Campaigns.Domain.Players;

public static class PlayerErrors
{
	public static Error NotFound(Guid playerId)
	{
		return Error.NotFound(
			"Player.NotFound",
			$"The player with identifier {playerId} was not found");
	}
}