using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TavernTrashers.Api.Common.Infrastructure.Modules;
using TavernTrashers.Api.Common.SourceGenerators;
using TavernTrashers.Api.Modules.Dice.Domain.DiceEngine;
using TavernTrashers.Api.Modules.Dice.Domain.Rolls;
using TavernTrashers.Api.Modules.Dice.Infrastructure.Database;
using TavernTrashers.Api.Modules.Dice.Infrastructure.Dice;
using TavernTrashers.Api.Modules.Dice.Infrastructure.Inbox;
using TavernTrashers.Api.Modules.Dice.Infrastructure.Outbox;
using Module = TavernTrashers.Api.Common.Infrastructure.Modules.Module;

namespace TavernTrashers.Api.Modules.Dice.Infrastructure;

[GenerateModuleBoilerplate(ModuleName, ModuleSchema)]
public class DiceModule : Module
{
	public const string ModuleName = "Dice";
	public const string ModuleSchema = "dice";
	public override string Name => ModuleName;
	public override string Schema => ModuleSchema;

	public override Assembly ApplicationAssembly => Application.AssemblyReference.Assembly;
	public override Assembly PresentationAssembly => Presentation.AssemblyReference.Assembly;

	protected override void AddDatabase(IHostApplicationBuilder builder) =>
		builder
		   .AddStandardModuleDatabase<DiceDbContext>(Name, Schema)
		   .AddScoped<IRollRepository, RollRepository>();

	protected override void ConfigureServices(IHostApplicationBuilder builder)
	{
		ConfigureOutbox<OutboxOptions, ProcessOutboxJob, ConfigureProcessOutboxJob>(builder);
		ConfigureInbox<InboxOptions, ProcessInboxJob, ConfigureProcessInboxJob>(builder);

		builder.Services.AddSingleton<IDiceEngine, DefaultDiceEngine>();
	}
}