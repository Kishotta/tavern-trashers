using FluentValidation;

namespace TavernTrashers.Api.Modules.Campaigns.Application.Campaigns.DeleteCampaign;

internal sealed class DeleteCampaignCommandValidator : AbstractValidator<DeleteCampaignCommand>
{
	public DeleteCampaignCommandValidator()
	{
		RuleFor(x => x.CampaignId)
		   .NotNull();
	}
}