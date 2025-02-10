using Microsoft.EntityFrameworkCore;
using TavernTrashers.Api.Modules.Campaigns.Application.Abstractions.Data;
using TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns;
using TavernTrashers.Api.Modules.Campaigns.Domain.Choices;
using TavernTrashers.Api.Modules.Campaigns.Domain.Questionnaires;
using TavernTrashers.Api.Modules.Campaigns.Domain.Questions;

namespace TavernTrashers.Api.Modules.Campaigns.Infrastructure.Database;

public class CampaignsDbContext(DbContextOptions<CampaignsDbContext> options)
	: DbContext(options), IUnitOfWork
{
	public DbSet<Campaign> Campaigns => Set<Campaign>();
	public DbSet<Choice> Choices => Set<Choice>();
	public DbSet<Question> Questions => Set<Question>();
	public DbSet<Questionnaire> Questionnaires => Set<Questionnaire>();
	
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.HasDefaultSchema(new CampaignsModule().Schema);

		modelBuilder.ApplyConfigurationsFromAssembly(Common.Infrastructure.AssemblyReference.Assembly);
		modelBuilder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly);
		
		base.OnModelCreating(modelBuilder);
	}
}