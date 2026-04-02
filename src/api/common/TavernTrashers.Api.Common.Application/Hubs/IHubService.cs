namespace TavernTrashers.Api.Common.Application.Hubs;

public interface IHubService
{
	Task PublishAsync<TPayload>(
		string group,
		string method,
		TPayload payload,
		CancellationToken cancellationToken = default);
}
