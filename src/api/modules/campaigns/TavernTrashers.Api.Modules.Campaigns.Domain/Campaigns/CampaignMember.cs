using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns;

public class CampaignMember
{
	private CampaignMember() { }
	public Guid PlayerId { get; private set; }
	public CampaignRole Role { get; private set; }
	public MembershipStatus Status { get; private set; }

	public static CampaignMember DungeonMaster(Guid playerId)
	{
		var campaignMember = new CampaignMember
		{
			PlayerId = playerId,
			Role     = CampaignRole.DungeonMaster,
			Status   = MembershipStatus.Joined,
		};

		return campaignMember;
	}

	public static CampaignMember Player(Guid playerId)
	{
		var campaignMember = new CampaignMember
		{
			PlayerId = playerId,
			Role     = CampaignRole.Player,
			Status   = MembershipStatus.Invited,
		};

		return campaignMember;
	}

	public Result AcceptInvitation()
	{
		if (Status != MembershipStatus.Invited)
			return CampaignMemberErrors.OnlyPendingInvitationsCanBeAccepted;

		Status = MembershipStatus.Joined;

		return Unit.Value;
	}

	public void Revoke() => Status = MembershipStatus.Revoked;
}