using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TavernTrashers.Api.Common.Infrastructure.Auditing;
using TavernTrashers.Api.Common.Infrastructure.Outbox;

namespace TavernTrashers.Api.Common.Infrastructure.Database;

public static class Postgres
{
	public static Action<IServiceProvider, DbContextOptionsBuilder> StandardOptions(IConfiguration configuration, string schema) =>
		(serviceProvider, options) =>
			options.UseNpgsql(
					configuration.GetConnectionString("Database")!,
					optionsBuilder =>
					{
						optionsBuilder.MigrationsHistoryTable(HistoryRepository.DefaultTableName, schema);
					})
			   .UseSnakeCaseNamingConvention()
			   .AddInterceptors(
					serviceProvider.GetRequiredService<InsertOutboxMessagesInterceptor>(),
					serviceProvider.GetRequiredService<WriteAuditLogInterceptor>());
}