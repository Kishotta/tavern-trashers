using TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns;

namespace TavernTrashers.Api.Modules.Campaigns.Application.Campaigns;

public sealed record InvitationResponse(
	Guid Id,
	string Email,
	CampaignRole Role)
{
	public static implicit operator InvitationResponse(Invitation invitation) =>
		new(invitation.Id,
			invitation.Email,
			invitation.Role);
}

public sealed record MyInvitationResponse(
	Guid Id,
	string Email,
	Guid CampaignId,
	string CampaignTitle,
	CampaignRole Role);