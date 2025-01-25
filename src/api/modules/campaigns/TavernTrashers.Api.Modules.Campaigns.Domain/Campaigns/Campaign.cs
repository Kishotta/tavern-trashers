using TavernTrashers.Api.Common.Domain.Auditing;
using TavernTrashers.Api.Common.Domain.Entities;

namespace TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns;

[Auditable]
public class Campaign : Entity<CampaignId>
{
	public string Name { get; private set; } = string.Empty;
	public string Description { get; private set; } = string.Empty;
	
	private Campaign() { }

	public static Campaign Create(
		string name,
		string description)
	{
		var campaign = new Campaign
		{
			Id          = CampaignId.Create(),
			Name        = name,
			Description = description
		};
		
		campaign.RaiseDomainEvent(new CampaignCreatedDomainEvent(campaign.Id));

		return campaign;
	}
}