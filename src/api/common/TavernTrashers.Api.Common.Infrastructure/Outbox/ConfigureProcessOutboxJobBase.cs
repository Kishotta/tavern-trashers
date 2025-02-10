using Microsoft.Extensions.Options;
using Quartz;

namespace TavernTrashers.Api.Common.Infrastructure.Outbox;

public class ConfigureProcessOutboxJobBase<TOutboxOptions, TProcessOutboxJob>(IOptions<TOutboxOptions> outboxOptions)
	: IConfigureOptions<QuartzOptions>
	where TOutboxOptions : OutboxOptionsBase
where TProcessOutboxJob : ProcessOutboxJobBase
{
	private readonly TOutboxOptions _outboxOptions = outboxOptions.Value;

	public void Configure(QuartzOptions options)
	{
		var jobName = typeof(TProcessOutboxJob).FullName!;

		options.AddJob<TProcessOutboxJob>(configure => configure.WithIdentity(jobName))
		   .AddTrigger(configure =>
				configure
				   .ForJob(jobName)
				   .WithSimpleSchedule(schedule =>
						schedule.WithInterval(TimeSpan.FromMilliseconds(_outboxOptions.IntervalInMilliseconds))
						   .RepeatForever()));
	}
}