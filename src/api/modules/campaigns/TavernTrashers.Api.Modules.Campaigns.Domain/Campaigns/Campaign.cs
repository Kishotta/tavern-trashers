using TavernTrashers.Api.Common.Domain.Auditing;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns.DomainEvents;

namespace TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns;

[Auditable]
public class Campaign : Entity
{
	private readonly List<Invitation> _invitations = [];
	private Campaign() { }

	public string Title { get; private set; } = string.Empty;
	public string Description { get; private set; } = string.Empty;

	public IReadOnlyCollection<Invitation> Invitations => _invitations.AsReadOnly();
	private bool PlayerInvitationSent(string email) => _invitations.All(invitation => invitation.Email != email);

	public static Result<Campaign> Create(
		string title,
		string description)
	{
		var campaign = new Campaign
		{
			Id          = Guid.NewGuid(),
			Title       = title,
			Description = description,
		};

		campaign.RaiseDomainEvent(new CampaignCreatedDomainEvent(campaign.Id));

		return campaign;
	}

	public Result UpdateDetails(Guid requestingPlayerId, string title, string description) =>
		Unit.Value
		   .Do(() =>
			{
				Title       = title;
				Description = description;

				RaiseDomainEvent(new CampaignDetailsUpdatedDomainEvent(Id));
			});

	public Result<Invitation> InvitePlayer(string email) =>
		Unit.Value
		   .Ensure(() => PlayerInvitationSent(email), CampaignErrors.InvitationAlreadySent)
		   .Then(() => Invitation.Create(email, CampaignRole.Player, Title))
		   .Do(invitation => _invitations.Add(invitation));
}