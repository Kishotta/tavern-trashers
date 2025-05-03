using Microsoft.EntityFrameworkCore;
using TavernTrashers.Api.Modules.Dice.Application.Abstractions.Data;

namespace TavernTrashers.Api.Modules.Dice.Infrastructure.Database;

public class DiceDbContext(DbContextOptions<DiceDbContext> options)
	: DbContext(options), IUnitOfWork
{
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.HasDefaultSchema(new DiceModule().Schema);

		modelBuilder.ApplyConfigurationsFromAssembly(Common.Infrastructure.AssemblyReference.Assembly);
		modelBuilder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly);

		base.OnModelCreating(modelBuilder);
	}
}