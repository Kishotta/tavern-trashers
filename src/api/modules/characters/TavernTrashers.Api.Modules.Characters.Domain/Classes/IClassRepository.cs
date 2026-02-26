using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Modules.Characters.Domain.Classes;

public interface IClassRepository
{
	void Add(Class @class);

	Task<Result<Class>> GetAsync(Guid classId, CancellationToken cancellationToken = default);
	Task<IReadOnlyCollection<Class>> GetAllAsync(CancellationToken cancellationToken = default);
	Task<bool> ExistsAsync(string name, CancellationToken cancellationToken = default);
}
