using Microsoft.EntityFrameworkCore;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Dice.Domain.Rolls;
using TavernTrashers.Api.Modules.Dice.Infrastructure.Database;

namespace TavernTrashers.Api.Modules.Dice.Infrastructure.Dice;

internal sealed class RollRepository(DiceDbContext dbContext) : IRollRepository
{
	public void Add(Roll roll) => dbContext.Rolls.Add(roll);

	public async Task<Result<Roll>> GetAsync(Guid rollId, CancellationToken cancellationToken = default) =>
		await dbContext
		   .Rolls
		   .Include(roll => roll.Parent)
		   .Include(roll => roll.Children)
		   .SingleOrDefaultAsync(roll => roll.Id == rollId, cancellationToken)
		   .ToResultAsync(RollErrors.NotFound(rollId));

	public async Task<IReadOnlyCollection<Roll>> GetAsync(
		string? context,
		CancellationToken cancellationToken = default) =>
		await dbContext
		   .Rolls
		   .Include(roll => roll.Parent)
		   .Include(roll => roll.Children)
		   .Where(roll => roll.ContextJson == context)
		   .ToListAsync(cancellationToken);

	public async Task<Result<Roll>> GetReadOnlyAsync(Guid rollId, CancellationToken cancellationToken = default) =>
		await dbContext
		   .Rolls
		   .AsNoTracking()
		   .Include(roll => roll.Parent)
		   .Include(roll => roll.Children)
		   .SingleOrDefaultAsync(roll => roll.Id == rollId, cancellationToken)
		   .ToResultAsync(RollErrors.NotFound(rollId));

	public async Task<IReadOnlyCollection<Roll>> GetReadOnlyAsync(
		CancellationToken cancellationToken = default) =>
		await dbContext
		   .Rolls
		   .AsNoTracking()
		   .Include(roll => roll.Parent)
		   .Include(roll => roll.Children)
		   .ToListAsync(cancellationToken);
}