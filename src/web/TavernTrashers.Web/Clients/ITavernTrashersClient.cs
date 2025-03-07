using TavernTrashers.Web.Components;
using TavernTrashers.Web.Models;

namespace TavernTrashers.Web.Clients;

public interface ITavernTrashersClient
{
	ValueTask<List<Campaign>> GetCampaignsAsync(CancellationToken cancellationToken = default);
	ValueTask<Campaign?> CreateCampaignAsync(CreateCampaignRequest request, CancellationToken cancellationToken = default);
}