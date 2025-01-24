using FluentValidation;

namespace TavernTrashers.Api.Modules.Campaigns.Application.Campaigns.CreateCampaign;

internal sealed class CreateCampaignCommandValidator : AbstractValidator<CreateCampaignCommand>
{
	public CreateCampaignCommandValidator()
	{
		RuleFor(x => x.Name)
		   .NotEmpty()
		   .MaximumLength(100);

		RuleFor(x => x.Description)
		   .NotEmpty()
		   .MaximumLength(500);
	}
}