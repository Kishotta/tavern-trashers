using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TavernTrashers.Api.Common.Infrastructure.Database;
using TavernTrashers.Api.Common.SourceGenerators;
using TavernTrashers.Api.Modules.Dice.Application.Abstractions.Data;
using TavernTrashers.Api.Modules.Dice.Domain.DiceEngine;
using TavernTrashers.Api.Modules.Dice.Domain.Rolls;
using TavernTrashers.Api.Modules.Dice.Infrastructure.Database;
using TavernTrashers.Api.Modules.Dice.Infrastructure.Dice;
using TavernTrashers.Api.Modules.Dice.Infrastructure.Inbox;
using TavernTrashers.Api.Modules.Dice.Infrastructure.Outbox;
using Module = TavernTrashers.Api.Common.Infrastructure.Modules.Module;

namespace TavernTrashers.Api.Modules.Dice.Infrastructure;

[GenerateModuleBoilerplate(DiceModuleName, DiceModuleSchema)]
public class DiceModule : Module
{
	private const string DiceModuleName = "Dice";
	private const string DiceModuleSchema = "dice";
	public override string Name => DiceModuleName;
	public override string Schema => DiceModuleSchema;

	public override Assembly ApplicationAssembly => Application.AssemblyReference.Assembly;
	public override Assembly PresentationAssembly => Presentation.AssemblyReference.Assembly;

	protected override void AddDatabase(IHostApplicationBuilder builder) =>
		builder.Services
		   .AddDbContext<DiceDbContext>(Postgres.StandardOptions(builder.Configuration, Schema))
		   .AddScoped<IUnitOfWork>(serviceProvider => serviceProvider.GetRequiredService<DiceDbContext>())
		   .AddScoped<IRollRepository, RollRepository>();

	protected override void ConfigureServices(IHostApplicationBuilder builder)
	{
		ConfigureOutbox<OutboxOptions, ProcessOutboxJob, ConfigureProcessOutboxJob>(builder);
		ConfigureInbox<InboxOptions, ProcessInboxJob, ConfigureProcessInboxJob>(builder);

		builder.Services.AddSingleton<IDiceEngine, DefaultDiceEngine>();
	}
}