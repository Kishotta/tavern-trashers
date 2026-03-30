using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns;

public static class CampaignErrors
{
    public static Error NotFound(Guid campaignId) =>
        Error.NotFound("Campaign.NotFound", $"Campaign with ID '{campaignId}' was not found.");

    public static Error InvalidTitle() =>
        Error.Validation("Campaign.InvalidTitle", "Campaign title cannot be empty.");
}
