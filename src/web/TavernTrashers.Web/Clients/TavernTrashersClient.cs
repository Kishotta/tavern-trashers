using System.Net.Http.Json;
using TavernTrashers.Web.Models;

namespace TavernTrashers.Web.Clients;

public class TavernTrashersClient(HttpClient httpClient) : ITavernTrashersClient
{

	public async ValueTask<List<Campaign>> GetCampaignsAsync(CancellationToken cancellationToken = default) =>
		await httpClient
		   .GetFromJsonAsync<List<Campaign>>("campaigns", cancellationToken: cancellationToken) ?? [];

	public async ValueTask<Campaign?> CreateCampaignAsync(CreateCampaignRequest request, CancellationToken cancellationToken = default)
	{
		var response = await httpClient.PostAsJsonAsync("campaigns", request, cancellationToken);
		response.EnsureSuccessStatusCode();
		return await response.Content.ReadFromJsonAsync<Campaign>(cancellationToken);
	}
}