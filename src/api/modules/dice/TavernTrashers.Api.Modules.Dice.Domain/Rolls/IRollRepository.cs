using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Modules.Dice.Domain.Rolls;

public interface IRollRepository
{
	void Add(Roll roll);

	Task<Result<Roll>> GetAsync(Guid rollId, CancellationToken cancellationToken = default);
	Task<IReadOnlyCollection<Roll>> GetAsync(string? context, CancellationToken cancellationToken = default);
	Task<Result<Roll>> GetReadOnlyAsync(Guid rollId, CancellationToken cancellationToken = default);
	Task<IReadOnlyCollection<Roll>> GetReadOnlyAsync(CancellationToken cancellationToken = default);
}