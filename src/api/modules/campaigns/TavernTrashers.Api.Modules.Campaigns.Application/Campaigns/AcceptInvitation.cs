using FluentValidation;
using TavernTrashers.Api.Common.Application.Authentication;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Campaigns.Application.Abstractions.Data;
using TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns;

namespace TavernTrashers.Api.Modules.Campaigns.Application.Campaigns;

public sealed record AcceptInvitationCommand(Guid CampaignId)
	: ICommand<CampaignResponse>;

internal sealed class AcceptInvitationCommandHandler(
	IClaimsProvider claims,
	ICampaignRepository campaignRepository,
	IUnitOfWork unitOfWork)
	: ICommandHandler<AcceptInvitationCommand, CampaignResponse>
{
	public async Task<Result<CampaignResponse>> Handle(
		AcceptInvitationCommand command,
		CancellationToken cancellationToken) =>
		await campaignRepository.GetAsync(command.CampaignId, cancellationToken)
		   .DoAsync(campaign =>
			{
				campaign.AcceptInvitation(claims.UserId);

				unitOfWork.SaveChangesAsync(cancellationToken);
			})
		   .TransformAsync(campaign => (CampaignResponse)campaign);
}

internal sealed class AcceptInvitationCommandValidator : AbstractValidator<AcceptInvitationCommand> { }