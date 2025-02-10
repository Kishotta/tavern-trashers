using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using TavernTrashers.Api.Common.Application.Data;

namespace TavernTrashers.Api.Common.Infrastructure.Database;

internal static class DbConnectionFactoryExtensions
{
	internal static IServiceCollection AddDbConnectionFactory(this IServiceCollection services, string databaseConnectionString)
	{
		var npgsqlDataSource = new NpgsqlDataSourceBuilder(databaseConnectionString).Build();
		services.TryAddSingleton(npgsqlDataSource);

		services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();

		return services;
	}
}