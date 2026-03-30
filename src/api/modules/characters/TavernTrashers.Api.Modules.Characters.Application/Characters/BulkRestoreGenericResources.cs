using FluentValidation;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Modules.Characters.Domain.Characters;
using TavernTrashers.Api.Modules.Characters.Domain.Resources;

namespace TavernTrashers.Api.Modules.Characters.Application.Characters;

public sealed record BulkRestoreGenericResourcesCommand(Guid CampaignId, ResetTrigger Trigger) : ICommand;

internal sealed class BulkRestoreGenericResourcesCommandValidator : AbstractValidator<BulkRestoreGenericResourcesCommand>
{
	public BulkRestoreGenericResourcesCommandValidator()
	{
		RuleFor(x => x.CampaignId).NotEmpty();
		RuleFor(x => x.Trigger).IsInEnum();
	}
}

internal sealed class BulkRestoreGenericResourcesCommandHandler(ICharacterRepository characterRepository)
	: ICommandHandler<BulkRestoreGenericResourcesCommand>
{
	public async Task<Result> Handle(BulkRestoreGenericResourcesCommand command, CancellationToken cancellationToken)
	{
		var characters = await characterRepository.GetForCampaignTrackedAsync(command.CampaignId, cancellationToken);

		foreach (var character in characters)
			character.BulkRestoreGenericResources(command.Trigger);

		return Result.Success();
	}
}
