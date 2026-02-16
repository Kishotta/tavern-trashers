using Microsoft.EntityFrameworkCore;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Characters.Domain.Classes;
using TavernTrashers.Api.Modules.Characters.Infrastructure.Database;

namespace TavernTrashers.Api.Modules.Characters.Infrastructure.Classes;

internal sealed class CharacterClassRepository(CharactersDbContext dbContext) : ICharacterClassRepository
{
	public void Add(CharacterClass characterClass) => dbContext.CharacterClasses.Add(characterClass);

	public async Task<Result<CharacterClass>> GetAsync(Guid classId, CancellationToken cancellationToken = default) =>
		await dbContext
		   .CharacterClasses
		   .Include(cc => cc.ResourceDefinitions)
		   .ThenInclude(rd => rd.Allowances)
		   .SingleOrDefaultAsync(c => c.Id == classId, cancellationToken)
		   .ToResultAsync(CharacterClassErrors.NotFound(classId));

	public async Task<IReadOnlyCollection<CharacterClass>> GetAllAsync(CancellationToken cancellationToken = default) =>
		await dbContext
		   .CharacterClasses
		   .AsNoTracking()
		   .Include(cc => cc.ResourceDefinitions)
		   .ThenInclude(rd => rd.Allowances)
		   .ToListAsync(cancellationToken);

	public async Task<bool> ExistsAsync(string name, CancellationToken cancellationToken = default) =>
		await dbContext
		   .CharacterClasses
		   .AsNoTracking()
		   .AnyAsync(cc => cc.ResourceDefinitions.Any(rd => rd.Name == name), cancellationToken);
}