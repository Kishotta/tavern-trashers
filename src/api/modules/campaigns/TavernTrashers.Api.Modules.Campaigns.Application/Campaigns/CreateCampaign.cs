using FluentValidation;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns;

namespace TavernTrashers.Api.Modules.Campaigns.Application.Campaigns;

public sealed record CreateCampaignCommand(string Title, string Description)
	: ICommand<CampaignResponse>;

internal sealed class CreateCampaignCommandHandler(ICampaignRepository campaignRepository)
	: ICommandHandler<CreateCampaignCommand, CampaignResponse>
{
	public async Task<Result<CampaignResponse>> Handle(
		CreateCampaignCommand command,
		CancellationToken cancellationToken) =>
		await Task.FromResult(
			Campaign.Create(command.Title, command.Description)
			   .Do(campaignRepository.Add)
			   .Transform(campaign => (CampaignResponse)campaign));
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