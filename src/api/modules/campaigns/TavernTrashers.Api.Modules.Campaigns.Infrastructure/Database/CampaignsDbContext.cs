using System.Reflection;
using Microsoft.EntityFrameworkCore;
using TavernTrashers.Api.Common.Infrastructure.Modules;
using TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns;

namespace TavernTrashers.Api.Modules.Campaigns.Infrastructure.Database;

public class CampaignsDbContext(DbContextOptions<CampaignsDbContext> options)
    : ModuleDbContext<CampaignsDbContext>(CampaignsModule.ModuleSchema, options)
{
    protected override Assembly InfrastructureAssembly => AssemblyReference.Assembly;

    internal DbSet<Campaign> Campaigns => Set<Campaign>();
}
