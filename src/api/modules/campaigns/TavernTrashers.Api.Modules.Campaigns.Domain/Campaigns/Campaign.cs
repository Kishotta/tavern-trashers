using TavernTrashers.Api.Common.Domain.Auditing;
using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns;

[Auditable]
public class Campaign : Entity
{
	public string Name { get; private set; } = string.Empty;
	public string Description { get; private set; } = string.Empty;
	
	private Campaign() { }

	public static Result<Campaign> Create(
		string name,
		string description)
	{
		var campaign = new Campaign
		{
			Id          = Guid.NewGuid(),
			Name        = name,
			Description = description
		};
		
		campaign.RaiseDomainEvent(new CampaignCreatedDomainEvent(campaign.Id));

		return campaign;
	}
}