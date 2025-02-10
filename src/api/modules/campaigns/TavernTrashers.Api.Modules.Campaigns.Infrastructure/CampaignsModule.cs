using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TavernTrashers.Api.Common.Infrastructure.Database;
using TavernTrashers.Api.Modules.Campaigns.Application.Abstractions.Data;
using TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns;
using TavernTrashers.Api.Modules.Campaigns.Domain.Questionnaires;
using TavernTrashers.Api.Modules.Campaigns.Infrastructure.Campaigns;
using TavernTrashers.Api.Modules.Campaigns.Infrastructure.Database;
using TavernTrashers.Api.Modules.Campaigns.Infrastructure.Inbox;
using TavernTrashers.Api.Modules.Campaigns.Infrastructure.Outbox;
using TavernTrashers.Api.Modules.Campaigns.Infrastructure.Questionnaires;
using Module = TavernTrashers.Api.Common.Infrastructure.Modules.Module;

namespace TavernTrashers.Api.Modules.Campaigns.Infrastructure;

public class CampaignsModule : Module
{
	public override string Name => "Campaigns";
	public override string Schema => "campaigns";

	public override Assembly ApplicationAssembly => Application.AssemblyReference.Assembly;
	public override Assembly PresentationAssembly => Presentation.AssemblyReference.Assembly;
	
	public override Type IdempotentDomainEventHandlerType => typeof(IdempotentDomainEventHandler<>);
	public override Type IdempotentIntegrationEventHandlerType => typeof(IdempotentIntegrationEventHandler<>);
	
	protected override void AddDatabase(IHostApplicationBuilder builder)
	{
		builder.Services
		   .AddDbContext<CampaignsDbContext>(Postgres.StandardOptions(builder.Configuration, Schema))
		   .AddScoped<IUnitOfWork>(serviceProvider => serviceProvider.GetRequiredService<CampaignsDbContext>())
		   .AddScoped<ICampaignRepository, CampaignRepository>()
		   .AddScoped<IQuestionnaireRepository, QuestionnaireRepository>();
	}

	protected override void ConfigureServices(IHostApplicationBuilder builder)
	{
		builder.Services
		   .Configure<OutboxOptions>(builder.Configuration.GetSection($"{Name}:Outbox"))
		   .ConfigureOptions<ConfigureProcessOutboxJob>();
		
		// builder.Services
		//    .Configure<InboxOptions>(builder.Configuration.GetSection($"{Name}:Inbox"))
		//    .ConfigureOptions<ConfigureProcessInboxJob>()
	}
}