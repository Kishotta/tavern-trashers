using TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns;

namespace TavernTrashers.Api.Modules.Campaigns.Application.Campaigns;

public sealed record CampaignResponse(Guid Id, string Title, string Description)
{
	public static implicit operator CampaignResponse(Campaign campaign) =>
		new(campaign.Id, campaign.Title, campaign.Description);
}