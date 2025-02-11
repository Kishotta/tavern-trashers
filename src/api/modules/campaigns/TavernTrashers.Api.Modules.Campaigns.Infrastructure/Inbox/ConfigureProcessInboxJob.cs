using Microsoft.Extensions.Options;
using TavernTrashers.Api.Common.Infrastructure.Inbox;
using TavernTrashers.Api.Modules.Campaigns.Infrastructure.Inbox;

namespace TavernTrashers.Api.Modules.Campaigns.Infrastructure;

internal sealed class ConfigureProcessInboxJob(IOptions<InboxOptions> inboxOptions)
	: ConfigureProcessInboxJobBase<InboxOptions, ProcessInboxJob>(inboxOptions);