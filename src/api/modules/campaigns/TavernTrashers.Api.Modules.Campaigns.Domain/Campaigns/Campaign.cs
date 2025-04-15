using TavernTrashers.Api.Common.Domain.Auditing;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Modules.Campaigns.Domain.Players;

namespace TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns;

[Auditable]
public class Campaign : Entity
{
	private List<Player> _dungeonMasters = [];
	private Campaign() { }

	public IReadOnlyList<Player> DungeonMasters => _dungeonMasters.AsReadOnly();
	public string Name { get; private set; } = string.Empty;
	public string Description { get; private set; } = string.Empty;
	public string Setting { get; private set; } = string.Empty;

	public CampaignStatus Status { get; private set; } = CampaignStatus.Draft;

	public static Result<Campaign> Create(
		Player dungeonMaster,
		string name,
		string description,
		string setting)
	{
		var campaign = new Campaign
		{
			Id              = Guid.NewGuid(),
			Name            = name,
			Description     = description,
			Setting         = setting,
			Status          = CampaignStatus.Draft,
			_dungeonMasters = [dungeonMaster],
		};

		campaign.RaiseDomainEvent(new CampaignCreatedDomainEvent(campaign.Id));

		return campaign;
	}
}

public enum CampaignStatus
{
	Draft = 0,
	Active = 1,
	Paused = 2,
	Completed = 3,
	Archived = 4,
}