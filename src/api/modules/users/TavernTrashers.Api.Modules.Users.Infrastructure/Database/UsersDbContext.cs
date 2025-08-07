using System.Reflection;
using Microsoft.EntityFrameworkCore;
using TavernTrashers.Api.Common.Infrastructure.Modules;
using TavernTrashers.Api.Modules.Users.Domain.Users;

namespace TavernTrashers.Api.Modules.Users.Infrastructure.Database;

public class UsersDbContext(DbContextOptions<UsersDbContext> options)
	: ModuleDbContext<UsersDbContext>(UsersModule.ModuleSchema, options)
{
	protected override Assembly InfrastructureAssembly => AssemblyReference.Assembly;

	internal DbSet<User> Users => Set<User>();
}