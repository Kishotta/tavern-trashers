using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TavernTrashers.Api.Common.Application.Data;
using TavernTrashers.Api.Common.Infrastructure.Database;

namespace TavernTrashers.Api.Common.Infrastructure.Modules;

public static class DatabaseExtensions
{
	public static IServiceCollection AddStandardModuleDatabase<TDbContext>(
		this IHostApplicationBuilder builder,
		string moduleName,
		string moduleSchema)
		where TDbContext : ModuleDbContext<TDbContext> =>
		builder
		   .Services
		   .AddDbContextPool<TDbContext>(Postgres.StandardOptions(builder.Configuration, moduleSchema))
		   .AddKeyedScoped<IUnitOfWork>(moduleName, (serviceProvider, _) =>
				serviceProvider.GetRequiredService<TDbContext>());
}