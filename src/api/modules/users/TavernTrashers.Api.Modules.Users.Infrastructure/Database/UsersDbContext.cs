using Microsoft.EntityFrameworkCore;
using TavernTrashers.Api.Modules.Users.Application.Abstractions.Data;
using TavernTrashers.Api.Modules.Users.Domain.Users;

namespace TavernTrashers.Api.Modules.Users.Infrastructure.Database;

public class UsersDbContext(DbContextOptions<UsersDbContext> options)
	: DbContext(options), IUnitOfWork
{
	internal DbSet<User> Users => Set<User>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.HasDefaultSchema(new UsersModule().Schema);

		modelBuilder.ApplyConfigurationsFromAssembly(Common.Infrastructure.AssemblyReference.Assembly);
		modelBuilder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly);
		
		base.OnModelCreating(modelBuilder);
	}
}