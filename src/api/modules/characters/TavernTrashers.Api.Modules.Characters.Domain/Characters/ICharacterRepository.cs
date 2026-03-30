using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Modules.Characters.Domain.Characters;

public interface ICharacterRepository
{
	void Add(Character character);
	void Remove(Character character);

	Task<Result<Character>> GetAsync(Guid characterId, CancellationToken cancellationToken = default);
	Task<IReadOnlyCollection<Character>> GetAllAsync(CancellationToken cancellationToken = default);
	Task<IReadOnlyCollection<Character>> GetForCampaignAsync(Guid campaignId, CancellationToken cancellationToken = default);
}
