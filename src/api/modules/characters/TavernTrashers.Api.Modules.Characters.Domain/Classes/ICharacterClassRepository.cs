using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Modules.Characters.Domain.Classes;

public interface ICharacterClassRepository
{
	void Add(CharacterClass characterClass);

	Task<Result<CharacterClass>> GetAsync(Guid classId, CancellationToken cancellationToken = default);
	Task<IReadOnlyCollection<CharacterClass>> GetAllAsync(CancellationToken cancellationToken = default);
	Task<bool> ExistsAsync(string name, CancellationToken cancellationToken = default);
}
