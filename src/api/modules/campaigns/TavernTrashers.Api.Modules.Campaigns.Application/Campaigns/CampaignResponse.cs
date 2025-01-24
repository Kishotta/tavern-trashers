using TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns;

namespace TavernTrashers.Api.Modules.Campaigns.Application.Campaigns;

public sealed record CampaignResponse(Guid Id, string Name, string Description)
{
	public static implicit operator CampaignResponse(Campaign campaign) => 
		new(campaign.Id, campaign.Name, campaign.Description);
}