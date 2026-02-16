using Microsoft.EntityFrameworkCore;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Characters.Domain.Characters;
using TavernTrashers.Api.Modules.Characters.Infrastructure.Database;

namespace TavernTrashers.Api.Modules.Characters.Infrastructure.Characters;

internal sealed class CharacterRepository(CharactersDbContext dbContext) : ICharacterRepository
{
	public void Add(Character character) => dbContext.Characters.Add(character);

	public void Remove(Character character) => dbContext.Characters.Remove(character);

	public async Task<Result<Character>> GetAsync(Guid characterId, CancellationToken cancellationToken = default) =>
		await dbContext
		   .Characters
		   .Include(c => c.ClassLevels)
		   .ThenInclude(cl => cl.CharacterClass)
		   .ThenInclude(cc => cc.ResourceDefinitions)
		   .ThenInclude(rd => rd.Allowances)
		   .Include(c => c.Resources)
		   .ThenInclude(r => r.ResourceDefinition)
		   .SingleOrDefaultAsync(c => c.Id == characterId, cancellationToken)
		   .ToResultAsync(CharacterErrors.NotFound(characterId));

	public async Task<IReadOnlyCollection<Character>> GetAllAsync(CancellationToken cancellationToken = default) =>
		await dbContext
		   .Characters
		   .AsNoTracking()
		   .Include(c => c.ClassLevels)
		   .ThenInclude(cl => cl.CharacterClass)
		   .Include(c => c.Resources)
		   .ThenInclude(r => r.ResourceDefinition)
		   .ToListAsync(cancellationToken);
}
