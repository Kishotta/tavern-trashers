using System.Reflection;
using Microsoft.EntityFrameworkCore;
using TavernTrashers.Api.Common.Infrastructure.Modules;
using TavernTrashers.Api.Modules.Campaigns.Application.Abstractions.Data;
using TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns;
using TavernTrashers.Api.Modules.Campaigns.Domain.Members;

namespace TavernTrashers.Api.Modules.Campaigns.Infrastructure.Database;

public class CampaignsDbContext(DbContextOptions<CampaignsDbContext> options)
	: ModuleDbContext<CampaignsDbContext>(CampaignsModule.ModuleSchema, options), IUnitOfWork
{
	protected override Assembly InfrastructureAssembly => AssemblyReference.Assembly;

	public DbSet<Campaign> Campaigns => Set<Campaign>();
	public DbSet<Member> Members => Set<Member>();
}