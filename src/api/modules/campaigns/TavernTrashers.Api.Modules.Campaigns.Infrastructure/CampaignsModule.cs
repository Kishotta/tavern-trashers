using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TavernTrashers.Api.Common.Infrastructure.Database;
using TavernTrashers.Api.Modules.Campaigns.Application.Abstractions.Data;
using TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns;
using TavernTrashers.Api.Modules.Campaigns.Infrastructure.Campaigns;
using TavernTrashers.Api.Modules.Campaigns.Infrastructure.Database;
using Module = TavernTrashers.Api.Common.Infrastructure.Module;

namespace TavernTrashers.Api.Modules.Campaigns.Infrastructure;

public class CampaignsModule : Module
{
	public static string Name => "Campaigns";
	public static string Schema => "campaigns";
	
	public override Assembly ApplicationAssembly => Application.AssemblyReference.Assembly;
	public override Assembly PresentationAssembly => Presentation.AssemblyReference.Assembly;
	
	protected override void AddDatabase(IHostApplicationBuilder builder)
	{
		builder.Services
		   .AddDbContext<CampaignsDbContext>(Postgres.StandardOptions(builder.Configuration, Schema))
		   .AddScoped<IUnitOfWork>(serviceProvider => serviceProvider.GetRequiredService<CampaignsDbContext>())
		   .AddScoped<ICampaignRepository, CampaignRepository>();
	}
}