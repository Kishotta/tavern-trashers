using FluentValidation;
using TavernTrashers.Api.Common.Application.Authentication;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Campaigns.Application.Abstractions.Data;
using TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns;

namespace TavernTrashers.Api.Modules.Campaigns.Application.Campaigns;

public sealed record CreateCampaignCommand(string Title, string Description)
	: ICommand<CampaignResponse>;

internal sealed class CreateCampaignCommandHandler(
	IClaimsProvider claims,
	ICampaignRepository campaignRepository,
	IUnitOfWork unitOfWork)
	: ICommandHandler<CreateCampaignCommand, CampaignResponse>
{
	public async Task<Result<CampaignResponse>> Handle(
		CreateCampaignCommand command,
		CancellationToken cancellationToken) =>
		await Campaign.Create(claims.UserId, command.Title, command.Description)
		   .DoAsync(async campaign =>
			{
				campaignRepository.Add(campaign);

				await unitOfWork.SaveChangesAsync(cancellationToken);
			})
		   .TransformAsync(campaign => (CampaignResponse)campaign);
}

internal sealed class CreateCampaignCommandValidator : AbstractValidator<CreateCampaignCommand>
{
	public CreateCampaignCommandValidator()
	{
		RuleFor(x => x.Title)
		   .NotEmpty()
		   .MaximumLength(100);

		RuleFor(x => x.Description)
		   .NotEmpty()
		   .MaximumLength(500);
	}
}