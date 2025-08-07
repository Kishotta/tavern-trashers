using FluentValidation;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns;

namespace TavernTrashers.Api.Modules.Campaigns.Application.Campaigns;

public sealed record InvitePlayerCommand(Guid CampaignId, string Email)
	: ICommand<InvitationResponse>;

internal sealed class InvitePlayerCommandHandler(ICampaignRepository campaignRepository) : ICommandHandler<InvitePlayerCommand, InvitationResponse>
{
	public async Task<Result<InvitationResponse>> Handle(
		InvitePlayerCommand command,
		CancellationToken cancellationToken) =>
		await campaignRepository.GetAsync(command.CampaignId, cancellationToken)
		   .ThenAsync(campaign => campaign.InvitePlayer(command.Email))
		   .TransformAsync(invitation => (InvitationResponse)invitation);
}

internal sealed class InvitePlayerCommandValidator : AbstractValidator<InvitePlayerCommand>
{
	public InvitePlayerCommandValidator()
	{
		RuleFor(command => command.CampaignId).NotEmpty();
		RuleFor(command => command.Email).NotEmpty().EmailAddress();
	}
}