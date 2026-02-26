using Microsoft.EntityFrameworkCore;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Characters.Domain.Classes;
using TavernTrashers.Api.Modules.Characters.Infrastructure.Database;

namespace TavernTrashers.Api.Modules.Characters.Infrastructure.Classes;

internal sealed class ClassRepository(CharactersDbContext dbContext) : IClassRepository
{
	public void Add(Class @class) => dbContext.Classes.Add(@class);

	public async Task<Result<Class>> GetAsync(Guid classId, CancellationToken cancellationToken = default) =>
		await dbContext
		   .Classes
		   .SingleOrDefaultAsync(c => c.Id == classId && !c.IsDeleted, cancellationToken)
		   .ToResultAsync(ClassErrors.NotFound(classId));

	public async Task<IReadOnlyCollection<Class>> GetAllAsync(CancellationToken cancellationToken = default) =>
		await dbContext
		   .Classes
		   .AsNoTracking()
		   .Where(c => !c.IsDeleted)
		   .ToListAsync(cancellationToken);

	public async Task<bool> ExistsAsync(string name, CancellationToken cancellationToken = default) =>
		await dbContext
		   .Classes
		   .AsNoTracking()
		   .AnyAsync(c => c.Name == name && !c.IsDeleted, cancellationToken);
}
