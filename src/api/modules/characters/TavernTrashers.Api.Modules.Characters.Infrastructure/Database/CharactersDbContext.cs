using System.Reflection;
using Microsoft.EntityFrameworkCore;
using TavernTrashers.Api.Common.Infrastructure.Modules;
using TavernTrashers.Api.Modules.Characters.Domain.Characters;
using TavernTrashers.Api.Modules.Characters.Domain.Classes;
using TavernTrashers.Api.Modules.Characters.Domain.ClassLevels;
using TavernTrashers.Api.Modules.Characters.Domain.Resources;

namespace TavernTrashers.Api.Modules.Characters.Infrastructure.Database;

public class CharactersDbContext(DbContextOptions<CharactersDbContext> options)
	: ModuleDbContext<CharactersDbContext>(CharactersModule.ModuleSchema, options)
{
	protected override Assembly InfrastructureAssembly => AssemblyReference.Assembly;

	internal DbSet<Character> Characters => Set<Character>();
	internal DbSet<CharacterClass> CharacterClasses => Set<CharacterClass>();
	internal DbSet<ClassLevel> ClassLevels => Set<ClassLevel>();
	internal DbSet<CharacterResource> CharacterResources => Set<CharacterResource>();
	internal DbSet<ResourceDefinition> ResourceDefinitions => Set<ResourceDefinition>();
}
