using TavernTrashers.Api.Common.Domain.Auditing;
using TavernTrashers.Api.Modules.Campaigns.Domain.Questionnaires;

namespace TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns;

[Auditable]
public class Campaign : Entity
{
	public string Name { get; private set; } = string.Empty;
	public string Description { get; private set; } = string.Empty;
	
	public IReadOnlyCollection<Questionnaire> Questionnaires => questionnaires.AsReadOnly();
	private readonly List<Questionnaire> questionnaires = [];
	
	private Campaign() { }

	public static Campaign Create(
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