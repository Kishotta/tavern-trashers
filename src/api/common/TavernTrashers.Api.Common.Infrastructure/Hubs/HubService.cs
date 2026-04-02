using Microsoft.AspNetCore.SignalR;
using TavernTrashers.Api.Common.Application.Hubs;

namespace TavernTrashers.Api.Common.Infrastructure.Hubs;

internal sealed class HubService(IHubContext<TavernTrashersHub> hubContext) : IHubService
{
	public Task PublishAsync<TPayload>(
		string group,
		string method,
		TPayload payload,
		CancellationToken cancellationToken = default) =>
		hubContext.Clients.Group(group).SendAsync(method, payload, cancellationToken);
}
