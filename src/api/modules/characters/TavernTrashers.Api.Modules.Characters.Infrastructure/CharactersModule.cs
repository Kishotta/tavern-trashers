using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TavernTrashers.Api.Common.Infrastructure.Database;
using TavernTrashers.Api.Common.SourceGenerators;
using TavernTrashers.Api.Modules.Characters.Application.Abstractions.Data;
using TavernTrashers.Api.Modules.Characters.Infrastructure.Database;
using TavernTrashers.Api.Modules.Characters.Infrastructure.Inbox;
using TavernTrashers.Api.Modules.Characters.Infrastructure.Outbox;
using Module = TavernTrashers.Api.Common.Infrastructure.Modules.Module;

namespace TavernTrashers.Api.Modules.Characters.Infrastructure;

[GenerateModuleBoilerplate(ModuleName, ModuleSchema)]
public class CharactersModule : Module
{
	public const string ModuleName = "Characters";
	public const string ModuleSchema = "characters";
	public override string Name => ModuleName;
	public override string Schema => ModuleSchema;

	public override Assembly ApplicationAssembly => Application.AssemblyReference.Assembly;
	public override Assembly PresentationAssembly => Presentation.AssemblyReference.Assembly;

	protected override void AddDatabase(IHostApplicationBuilder builder) =>
		builder.Services
		   .AddDbContext<CharactersDbContext>(Postgres.StandardOptions(builder.Configuration, Schema))
		   .AddScoped<IUnitOfWork>(serviceProvider => serviceProvider.GetRequiredService<CharactersDbContext>());

	protected override void ConfigureServices(IHostApplicationBuilder builder)
	{
		ConfigureOutbox<OutboxOptions, ProcessOutboxJob, ConfigureProcessOutboxJob>(builder);
		ConfigureInbox<InboxOptions, ProcessInboxJob, ConfigureProcessInboxJob>(builder);
	}
}