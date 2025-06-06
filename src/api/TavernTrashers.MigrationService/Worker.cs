using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using TavernTrashers.Api.Modules.Campaigns.Infrastructure.Database;
using TavernTrashers.Api.Modules.Dice.Infrastructure.Database;
using TavernTrashers.Api.Modules.Users.Infrastructure.Database;

namespace TavernTrashers.MigrationService;

public class Worker(
	IServiceProvider serviceProvider,
	IHostApplicationLifetime hostApplicationLifetime) : BackgroundService
{
	public const string ActivitySourceName = "Migrations";
	private static readonly ActivitySource ActivitySource = new(ActivitySourceName);

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		using var activity = ActivitySource.StartActivity("Migrating database", ActivityKind.Client);

		try
		{
			using var scope = serviceProvider.CreateScope();
			DbContext[] dbContexts =
			[
				scope.ServiceProvider.GetRequiredService<CampaignsDbContext>(),
				scope.ServiceProvider.GetRequiredService<DiceDbContext>(),
				scope.ServiceProvider.GetRequiredService<UsersDbContext>(),
			];

			foreach (var dbContext in dbContexts)
			{
				await EnsureDatabaseAsync(dbContext, stoppingToken);
				await RunMigrationAsync(dbContext, stoppingToken);
			}
		}
		catch (Exception ex)
		{
			activity?.AddException(ex);
			throw;
		}

		hostApplicationLifetime.StopApplication();
	}

	private static async Task EnsureDatabaseAsync(DbContext dbContext, CancellationToken cancellationToken)
	{
		var dbCreator = dbContext.GetService<IRelationalDatabaseCreator>();

		var strategy = dbContext.Database.CreateExecutionStrategy();
		await strategy.ExecuteAsync(async () =>
		{
			// Create the database if it does not exist.
			// Do this first so there is then a database to start a transaction against.
			if (!await dbCreator.ExistsAsync(cancellationToken)) await dbCreator.CreateAsync(cancellationToken);
		});
	}

	private static async Task RunMigrationAsync(DbContext dbContext, CancellationToken cancellationToken)
	{
		var strategy = dbContext.Database.CreateExecutionStrategy();
		await strategy.ExecuteAsync(async () => { await dbContext.Database.MigrateAsync(cancellationToken); });
	}
}