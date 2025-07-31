using System.Reflection;
using Microsoft.EntityFrameworkCore;
using TavernTrashers.Api.Common.Infrastructure.Modules;
using TavernTrashers.Api.Modules.Dice.Application.Abstractions.Data;
using TavernTrashers.Api.Modules.Dice.Domain.Rolls;

namespace TavernTrashers.Api.Modules.Dice.Infrastructure.Database;

public class DiceDbContext(DbContextOptions<DiceDbContext> options)
	: ModuleDbContext<DiceDbContext>(DiceModule.ModuleSchema, options), IUnitOfWork
{
	protected override Assembly InfrastructureAssembly => AssemblyReference.Assembly;

	internal DbSet<Roll> Rolls => Set<Roll>();
}