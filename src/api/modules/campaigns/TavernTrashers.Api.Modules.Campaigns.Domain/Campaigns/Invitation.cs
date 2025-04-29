namespace TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns;

public class Invitation
{
	private Invitation() { }

	public Guid Id { get; private set; }
	public string Email { get; private set; } = string.Empty;
	public CampaignRole Role { get; private set; }
	public string CampaignTitle { get; private set; } = string.Empty;

	public static Invitation Create(string email, CampaignRole role, string campaignTitle)
	{
		var invitation = new Invitation
		{
			Id            = Guid.NewGuid(),
			Email         = email,
			Role          = role,
			CampaignTitle = campaignTitle,
		};

		return invitation;
	}
}