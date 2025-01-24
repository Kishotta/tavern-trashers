using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns;

public static class CampaignErrors
{
	public static Error NotFound(CampaignId campaignId) =>
		Error.NotFound("Campaigns.NotFound", $"The campaign with the identifier {campaignId} was not found");
}