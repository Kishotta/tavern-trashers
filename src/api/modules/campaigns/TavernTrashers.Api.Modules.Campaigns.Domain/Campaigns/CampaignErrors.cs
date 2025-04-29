using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns;

public static class CampaignErrors
{
	public static readonly Error InvitationAlreadySent =
		Error.Conflict("Campaigns.InvitationAlreadySent", "An invitation is already pending for that address.");

	public static readonly Error NotFound =
		Error.NotFound("Campaigns.NotFound", "Campaign not found.");
}