using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns;

public static class CampaignMemberErrors
{
	public static readonly Error OnlyPendingInvitationsCanBeAccepted = Error.Problem(
		"CampaignMember.OnlyPendingInvitationsCanBeAccepted",
		"Only pending invitations can be accepted");

	public static readonly Error OnlyDungeonMasterCanPerformThisAction = Error.Authorization(
		"CampaignMember.OnlyDungeonMasterCanPerformThisAction",
		"Only the Dungeon Master can perform this action");

	public static readonly Error InviteeAlreadyAMember = Error.Problem(
		"CampaignMember.InviteeAlreadyAMember",
		"The invitee is already a member of the campaign");

	public static readonly Error InvitationAlreadySent = Error.Problem(
		"CampaignMember.InvitationAlreadySent",
		"The invitee has already been invited to the campaign");

	public static readonly Error InvitationNotFound = Error.NotFound(
		"CampaignMember.InvitationNotFound",
		"No invitation found");
}