using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Modules.Dice.Domain.Rolls;

public interface IRollRepository
{
	void Add(Roll roll);
	
	Task<Result<Roll>> GetAsync(Guid rollId, CancellationToken cancellationToken = default);
	Task<Result<IReadOnlyList<Roll>>> GetAsync(string? context, CancellationToken cancellationToken = default);
}