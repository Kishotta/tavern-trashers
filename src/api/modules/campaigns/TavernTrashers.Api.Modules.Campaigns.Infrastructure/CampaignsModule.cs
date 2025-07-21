using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TavernTrashers.Api.Common.Infrastructure.Database;
using TavernTrashers.Api.Common.SourceGenerators;
using TavernTrashers.Api.Modules.Campaigns.Application.Abstractions.Data;
using TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns;
using TavernTrashers.Api.Modules.Campaigns.Domain.Members;
using TavernTrashers.Api.Modules.Campaigns.Infrastructure.Campaigns;
using TavernTrashers.Api.Modules.Campaigns.Infrastructure.Database;
using TavernTrashers.Api.Modules.Campaigns.Infrastructure.Inbox;
using TavernTrashers.Api.Modules.Campaigns.Infrastructure.Members;
using TavernTrashers.Api.Modules.Campaigns.Infrastructure.Outbox;
using Module = TavernTrashers.Api.Common.Infrastructure.Modules.Module;

namespace TavernTrashers.Api.Modules.Campaigns.Infrastructure;

[GenerateModuleBoilerplate(CampaignsModuleName, CampaignsModuleSchema)]
public class CampaignsModule : Module
{
	private const string CampaignsModuleName = "Campaigns";
	private const string CampaignsModuleSchema = "campaigns";
	public override string Name => CampaignsModuleName;
	public override string Schema => CampaignsModuleSchema;

	public override Assembly ApplicationAssembly => Application.AssemblyReference.Assembly;
	public override Assembly PresentationAssembly => Presentation.AssemblyReference.Assembly;

	protected override void AddDatabase(IHostApplicationBuilder builder) =>
		builder.Services
		   .AddDbContext<CampaignsDbContext>(Postgres.StandardOptions(builder.Configuration, Schema))
		   .AddScoped<IUnitOfWork>(serviceProvider => serviceProvider.GetRequiredService<CampaignsDbContext>())
		   .AddScoped<ICampaignRepository, CampaignRepository>()
		   .AddScoped<IMemberRepository, MemberRepository>();

	protected override void ConfigureServices(IHostApplicationBuilder builder)
	{
		ConfigureOutbox<OutboxOptions, ProcessOutboxJob, ConfigureProcessOutboxJob>(builder);
		ConfigureInbox<InboxOptions, ProcessInboxJob, ConfigureProcessInboxJob>(builder);
	}
}