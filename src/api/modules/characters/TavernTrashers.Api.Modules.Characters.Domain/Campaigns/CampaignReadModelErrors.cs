using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Modules.Characters.Domain.Campaigns;

public static class CampaignReadModelErrors
{
	public static Error NotFound(Guid campaignId) =>
		Error.NotFound(
			"Characters.CampaignNotFound",
			$"Campaign with ID '{campaignId}' was not found.");
}
