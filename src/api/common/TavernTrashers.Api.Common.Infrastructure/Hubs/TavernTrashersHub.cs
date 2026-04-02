using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace TavernTrashers.Api.Common.Infrastructure.Hubs;

[Authorize]
public sealed class TavernTrashersHub : Hub
{
	public Task JoinGroupAsync(string groupName) =>
		Groups.AddToGroupAsync(Context.ConnectionId, groupName);

	public Task LeaveGroupAsync(string groupName) =>
		Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
}
