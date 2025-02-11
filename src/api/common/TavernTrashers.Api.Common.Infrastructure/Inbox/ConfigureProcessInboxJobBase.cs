using Microsoft.Extensions.Options;
using Quartz;

namespace TavernTrashers.Api.Common.Infrastructure.Inbox;

public class ConfigureProcessInboxJobBase<TInboxOptions, TProcessInboxJob>(IOptions<TInboxOptions> inboxOptions)
	: IConfigureOptions<QuartzOptions>
	where TInboxOptions : InboxOptionsBase
	where TProcessInboxJob : ProcessInboxJobBase
{
	private readonly TInboxOptions _inboxOptions = inboxOptions.Value;

	public void Configure(QuartzOptions options)
	{
		var jobName = typeof(TProcessInboxJob).FullName!;

		options.AddJob<TProcessInboxJob>(configure => configure.WithIdentity(jobName))
		   .AddTrigger(configure =>
				configure
				   .ForJob(jobName)
				   .WithSimpleSchedule(schedule =>
						schedule.WithInterval(TimeSpan.FromMilliseconds(_inboxOptions.IntervalInMilliseconds))
						   .RepeatForever()));
	}
}