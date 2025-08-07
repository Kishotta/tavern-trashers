using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TavernTrashers.Api.Common.Infrastructure.Modules;
using TavernTrashers.Api.Common.SourceGenerators;
using TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns;
using TavernTrashers.Api.Modules.Campaigns.Domain.Members;
using TavernTrashers.Api.Modules.Campaigns.Infrastructure.Campaigns;
using TavernTrashers.Api.Modules.Campaigns.Infrastructure.Database;
using TavernTrashers.Api.Modules.Campaigns.Infrastructure.Inbox;
using TavernTrashers.Api.Modules.Campaigns.Infrastructure.Members;
using TavernTrashers.Api.Modules.Campaigns.Infrastructure.Outbox;
using Module = TavernTrashers.Api.Common.Infrastructure.Modules.Module;

namespace TavernTrashers.Api.Modules.Campaigns.Infrastructure;

[GenerateModuleBoilerplate(ModuleName, ModuleSchema)]
public class CampaignsModule : Module
{
	public const string ModuleName = "Campaigns";
	public const string ModuleSchema = "campaigns";
	public override string Name => ModuleName;
	public override string Schema => ModuleSchema;

	public override Assembly ApplicationAssembly => Application.AssemblyReference.Assembly;
	public override Assembly PresentationAssembly => Presentation.AssemblyReference.Assembly;

	protected override void AddDatabase(IHostApplicationBuilder builder) =>
		builder
		   .AddStandardModuleDatabase<CampaignsDbContext>(Name, Schema)
		   .AddScoped<ICampaignRepository, CampaignRepository>()
		   .AddScoped<IMemberRepository, MemberRepository>();

	protected override void ConfigureServices(IHostApplicationBuilder builder)
	{
		ConfigureOutbox<OutboxOptions, ProcessOutboxJob, ConfigureProcessOutboxJob>(builder);
		ConfigureInbox<InboxOptions, ProcessInboxJob, ConfigureProcessInboxJob>(builder);
	}
}