using TavernTrashers.Api.Common.Domain.Entities;

namespace TavernTrashers.Api.Modules.Characters.Domain.Campaigns;

public sealed class CampaignReadModel : Entity
{
	private CampaignReadModel() { }

	public string Title { get; private set; } = string.Empty;

	public static CampaignReadModel Create(Guid campaignId, string title) =>
		new()
		{
			Id    = campaignId,
			Title = title.Trim(),
		};

	public void UpdateTitle(string title) => Title = title.Trim();
}
