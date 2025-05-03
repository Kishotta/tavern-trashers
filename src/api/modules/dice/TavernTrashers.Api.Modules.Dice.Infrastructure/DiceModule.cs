using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TavernTrashers.Api.Common.Infrastructure.Database;
using TavernTrashers.Api.Modules.Dice.Application.Abstractions.Data;
using TavernTrashers.Api.Modules.Dice.Domain.DiceEngine;
using TavernTrashers.Api.Modules.Dice.Domain.Rolls;
using TavernTrashers.Api.Modules.Dice.Infrastructure.Database;
using TavernTrashers.Api.Modules.Dice.Infrastructure.Dice;
using TavernTrashers.Api.Modules.Dice.Infrastructure.Inbox;
using TavernTrashers.Api.Modules.Dice.Infrastructure.Outbox;
using Module = TavernTrashers.Api.Common.Infrastructure.Modules.Module;

namespace TavernTrashers.Api.Modules.Dice.Infrastructure;

public class DiceModule : Module
{
	public override string Name => "Dice";
	public override string Schema => "dice";

	public override Assembly ApplicationAssembly => Application.AssemblyReference.Assembly;
	public override Assembly PresentationAssembly => Presentation.AssemblyReference.Assembly;

	public override Type IdempotentDomainEventHandlerType => typeof(IdempotentDomainEventHandler<>);
	public override Type IdempotentIntegrationEventHandlerType => typeof(IdempotentIntegrationEventHandler<>);
	public override Type IntegrationEventConsumerType => typeof(IntegrationEventConsumer<>);

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