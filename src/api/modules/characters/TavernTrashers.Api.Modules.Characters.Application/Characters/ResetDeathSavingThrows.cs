using FluentValidation;
using TavernTrashers.Api.Common.Application.Authentication;
using TavernTrashers.Api.Common.Application.Hubs;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Characters.Application.Hubs;
using TavernTrashers.Api.Modules.Characters.Domain.Characters;

namespace TavernTrashers.Api.Modules.Characters.Application.Characters;

public sealed record ResetDeathSavingThrowsCommand(Guid CharacterId) : ICommand<DeathSavingThrowsResponse>;

internal sealed class ResetDeathSavingThrowsCommandValidator : AbstractValidator<ResetDeathSavingThrowsCommand>
{
	public ResetDeathSavingThrowsCommandValidator()
	{
		RuleFor(x => x.CharacterId).NotEmpty();
	}
}

internal sealed class ResetDeathSavingThrowsCommandHandler(
	ICharacterRepository characterRepository,
	IHubService hubService,
	IClaimsProvider claimsProvider)
	: ICommandHandler<ResetDeathSavingThrowsCommand, DeathSavingThrowsResponse>
{
	public async Task<Result<DeathSavingThrowsResponse>> Handle(
		ResetDeathSavingThrowsCommand command,
		CancellationToken cancellationToken)
	{
		var characterResult = await characterRepository.GetAsync(command.CharacterId, cancellationToken);
		if (characterResult.IsFailure) return characterResult.Error;

		var character = characterResult.Value;
		var oldSuccesses = character.DeathSavingThrows.Successes;
		var oldFailures = character.DeathSavingThrows.Failures;

		character.ResetDeathSavingThrows();

		await hubService.PublishAsync(
			$"campaign:{character.CampaignId}",
			"ResourceChanged",
			new ResourceChangedNotification(
				character.Id,
				character.Name,
				character.CampaignId,
				"Death Saving Throws",
				$"S:{oldSuccesses}/F:{oldFailures}",
				$"S:{character.DeathSavingThrows.Successes}/F:{character.DeathSavingThrows.Failures}",
				claimsProvider.GetEmail()),
			cancellationToken);

		return (DeathSavingThrowsResponse)character.DeathSavingThrows;
	}
}
