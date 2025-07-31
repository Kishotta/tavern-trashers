using System.Reflection;
using Microsoft.EntityFrameworkCore;
using TavernTrashers.Api.Common.Infrastructure.Modules;
using TavernTrashers.Api.Modules.Characters.Application.Abstractions.Data;

namespace TavernTrashers.Api.Modules.Characters.Infrastructure.Database;

public class CharactersDbContext(DbContextOptions<CharactersDbContext> options)
	: ModuleDbContext<CharactersDbContext>(CharactersModule.ModuleSchema, options), IUnitOfWork
{
	protected override Assembly InfrastructureAssembly => AssemblyReference.Assembly;
}