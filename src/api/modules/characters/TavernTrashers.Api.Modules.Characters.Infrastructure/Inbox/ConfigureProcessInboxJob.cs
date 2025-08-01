using Microsoft.Extensions.Options;
using TavernTrashers.Api.Common.Infrastructure.Inbox;

namespace TavernTrashers.Api.Modules.Characters.Infrastructure.Inbox;

internal sealed class ConfigureProcessInboxJob(IOptions<InboxOptions> inboxOptions)
	: ConfigureProcessInboxJobBase<InboxOptions, ProcessInboxJob>(inboxOptions);