using FluentValidation;
using TavernTrashers.Api.Common.Application.Authentication;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Campaigns.Application.Abstractions.Data;
using TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns;

namespace TavernTrashers.Api.Modules.Campaigns.Application.Campaigns;

public sealed record UpdateCampaignCommand(Guid CampaignId, string Title, string Description)
	: ICommand<CampaignResponse>;

internal sealed class UpdateCampaignCommandHandler(
	IClaimsProvider claims,
	ICampaignRepository campaignRepository,
	IUnitOfWork unitOfWork)
	: ICommandHandler<UpdateCampaignCommand, CampaignResponse>
{
	public async Task<Result<CampaignResponse>> Handle(
		UpdateCampaignCommand command,
		CancellationToken cancellationToken) =>
		await campaignRepository.GetAsync(command.CampaignId, cancellationToken)
		   .DoAsync(async campaign =>
			{
				campaign.UpdateDetails(claims.UserId, command.Title, command.Description);

				await unitOfWork.SaveChangesAsync(cancellationToken);
			})
		   .TransformAsync(campaign => (CampaignResponse)campaign);
}

internal sealed class UpdateCampaignCommandValidator : AbstractValidator<UpdateCampaignCommand>
{
	public UpdateCampaignCommandValidator()
	{
		RuleFor(x => x.Title)
		   .NotEmpty()
		   .MaximumLength(100);

		RuleFor(x => x.Description)
		   .NotEmpty()
		   .MaximumLength(500);
	}
}