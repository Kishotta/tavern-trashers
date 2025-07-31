using System.Reflection;
using Microsoft.EntityFrameworkCore;
using TavernTrashers.Api.Common.Application.Data;

namespace TavernTrashers.Api.Common.Infrastructure.Modules;

public abstract class ModuleDbContext<TModuleDbContext>(
	string moduleSchema,
	DbContextOptions<TModuleDbContext> options) : DbContext(options), IUnitOfWorkBase
	where TModuleDbContext : ModuleDbContext<TModuleDbContext>
{
	protected abstract Assembly InfrastructureAssembly { get; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.HasDefaultSchema(moduleSchema);

		modelBuilder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly);
		modelBuilder.ApplyConfigurationsFromAssembly(InfrastructureAssembly);

		base.OnModelCreating(modelBuilder);
	}
}