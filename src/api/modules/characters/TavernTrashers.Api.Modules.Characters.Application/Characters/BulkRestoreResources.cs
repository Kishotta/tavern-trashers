using FluentValidation;
using TavernTrashers.Api.Common.Application.Authentication;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Modules.Characters.Domain.Characters;
using TavernTrashers.Api.Modules.Characters.Domain.Resources;

namespace TavernTrashers.Api.Modules.Characters.Application.Characters;

public sealed record BulkRestoreResourcesCommand(Guid CampaignId, ResetTrigger Trigger) : ICommand;

internal sealed class BulkRestoreResourcesCommandValidator : AbstractValidator<BulkRestoreResourcesCommand>
{
	public BulkRestoreResourcesCommandValidator()
	{
		RuleFor(x => x.CampaignId).NotEmpty();
	}
}

internal sealed class BulkRestoreResourcesCommandHandler(
	ICharacterRepository characterRepository,
	IClaimsProvider claimsProvider)
	: ICommandHandler<BulkRestoreResourcesCommand>
{
	public async Task<Result> Handle(BulkRestoreResourcesCommand command, CancellationToken cancellationToken)
	{
		var characters = await characterRepository.GetForCampaignAsync(command.CampaignId, cancellationToken);
		var actor = claimsProvider.GetEmail();

		foreach (var character in characters)
			character.BulkRestoreByTrigger(command.Trigger, actor);

		return Result.Success();
	}
}
