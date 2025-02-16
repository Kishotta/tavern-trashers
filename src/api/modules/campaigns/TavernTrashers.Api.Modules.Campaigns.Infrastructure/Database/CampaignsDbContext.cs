using Microsoft.EntityFrameworkCore;
using TavernTrashers.Api.Modules.Campaigns.Application.Abstractions.Data;
using TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns;

namespace TavernTrashers.Api.Modules.Campaigns.Infrastructure.Database;

public class CampaignsDbContext(DbContextOptions<CampaignsDbContext> options)
	: DbContext(options), IUnitOfWork
{
	public DbSet<Campaign> Campaigns => Set<Campaign>();
	
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.HasDefaultSchema(new CampaignsModule().Schema);

		modelBuilder.ApplyConfigurationsFromAssembly(Common.Infrastructure.AssemblyReference.Assembly);
		modelBuilder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly);
		
		base.OnModelCreating(modelBuilder);
	}
}