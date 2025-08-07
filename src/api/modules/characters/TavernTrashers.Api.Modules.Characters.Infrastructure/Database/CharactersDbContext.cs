using System.Reflection;
using Microsoft.EntityFrameworkCore;
using TavernTrashers.Api.Common.Infrastructure.Modules;

namespace TavernTrashers.Api.Modules.Characters.Infrastructure.Database;

public class CharactersDbContext(DbContextOptions<CharactersDbContext> options)
	: ModuleDbContext<CharactersDbContext>(CharactersModule.ModuleSchema, options)
{
	protected override Assembly InfrastructureAssembly => AssemblyReference.Assembly;
}