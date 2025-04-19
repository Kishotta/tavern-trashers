using FluentValidation;
using TavernTrashers.Api.Common.Application.Authentication;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Campaigns.Application.Abstractions.Data;
using TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns;

namespace TavernTrashers.Api.Modules.Campaigns.Application.Campaigns;

public sealed record InvitePlayerCommand(Guid CampaignId, Guid InviteeId)
	: ICommand<CampaignResponse>;

internal sealed class InvitePlayerCommandHandler(
	IClaimsProvider claims,
	ICampaignRepository campaignRepository,
	IUnitOfWork unitOfWork)
	: ICommandHandler<InvitePlayerCommand, CampaignResponse>
{
	public async Task<Result<CampaignResponse>> Handle(
		InvitePlayerCommand command,
		CancellationToken cancellationToken) =>
		await campaignRepository.GetAsync(command.CampaignId, cancellationToken)
		   .DoAsync(async campaign =>
			{
				campaign.InvitePlayer(claims.UserId, command.InviteeId);

				await unitOfWork.SaveChangesAsync(cancellationToken);
			})
		   .TransformAsync(campaign => (CampaignResponse)campaign);
}

internal sealed class InvitePlayerCommandValidator : AbstractValidator<InvitePlayerCommand> { }