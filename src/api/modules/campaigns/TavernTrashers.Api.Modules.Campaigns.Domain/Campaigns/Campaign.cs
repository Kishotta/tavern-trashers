using TavernTrashers.Api.Common.Domain.Auditing;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;

namespace TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns;

[Auditable]
public class Campaign : Entity
{
	private readonly List<CampaignMember> _members = [];
	private Campaign() { }

	public string Title { get; private set; } = string.Empty;
	public string Description { get; private set; } = string.Empty;

	public IReadOnlyList<CampaignMember> Members => _members.AsReadOnly();

	public static Result<Campaign> Create(
		Guid dungeonMasterId,
		string title,
		string description)
	{
		var campaign = new Campaign
		{
			Id          = Guid.NewGuid(),
			Title       = title,
			Description = description,
		};

		campaign._members.Add(CampaignMember.DungeonMaster(dungeonMasterId));

		campaign.RaiseDomainEvent(new CampaignCreatedDomainEvent(campaign.Id));

		return campaign;
	}

	public Result UpdateDetails(Guid requestingPlayerId, string title, string description) =>
		EnsureRequesterIsDungeonMaster(requestingPlayerId)
		   .Do(() =>
			{
				Title       = title;
				Description = description;

				RaiseDomainEvent(new CampaignDetailsUpdatedDomainEvent(Id));
			});

	public Result InvitePlayer(Guid requestingPlayerId, Guid inviteeId) =>
		EnsureRequesterIsDungeonMaster(requestingPlayerId)
		   .Ensure(() => !PlayerIsMemberOfCampaign(inviteeId),
				CampaignMemberErrors.InviteeAlreadyAMember)
		   .Ensure(() => !PlayerHasOpenCampaignInvitation(inviteeId),
				CampaignMemberErrors.InvitationAlreadySent)
		   .Do(() =>
			{
				_members.Add(CampaignMember.Player(inviteeId));

				RaiseDomainEvent(new CampaignPlayerInvitedDomainEvent(Id, inviteeId));
			});

	public Result AcceptInvitation(Guid playerId) =>
		_members.SingleOrDefault(member => PlayerHasOpenCampaignInvitation(member.PlayerId))
		   .ToResult(CampaignMemberErrors.InvitationNotFound)
		   .Do(membership =>
			{
				membership.AcceptInvitation();

				RaiseDomainEvent(new CampaignPlayerJoinedDomainEvent(Id, playerId));
			});

	public bool PlayerIsMemberOfCampaign(Guid inviteeId) =>
		_members.Any(member => member.PlayerId == inviteeId &&
		                       member.Status == MembershipStatus.Joined);

	private bool PlayerHasOpenCampaignInvitation(Guid inviteeId) =>
		_members.Any(member => member.PlayerId == inviteeId &&
		                       member.Status == MembershipStatus.Invited);

	public Result EnsureRequesterIsDungeonMaster(Guid requestingPlayerId) =>
		_members.Any(member => member.PlayerId == requestingPlayerId &&
		                       member.Role == CampaignRole.DungeonMaster &&
		                       member.Status != MembershipStatus.Joined)
			? Unit.Value
			: CampaignMemberErrors.OnlyDungeonMasterCanPerformThisAction;
}