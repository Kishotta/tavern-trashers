using System.Reflection;
using Microsoft.EntityFrameworkCore;
using TavernTrashers.Api.Common.Infrastructure.Modules;
using TavernTrashers.Api.Modules.Users.Application.Abstractions.Data;
using TavernTrashers.Api.Modules.Users.Domain.Users;

namespace TavernTrashers.Api.Modules.Users.Infrastructure.Database;

public class UsersDbContext(DbContextOptions<UsersDbContext> options)
	: ModuleDbContext<UsersDbContext>(UsersModule.ModuleSchema, options), IUnitOfWork
{
	protected override Assembly InfrastructureAssembly => AssemblyReference.Assembly;

	internal DbSet<User> Users => Set<User>();
}