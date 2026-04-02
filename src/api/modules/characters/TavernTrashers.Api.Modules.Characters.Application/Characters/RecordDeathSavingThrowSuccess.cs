using FluentValidation;
using TavernTrashers.Api.Common.Application.Authentication;
using TavernTrashers.Api.Common.Application.Hubs;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Characters.Application.Hubs;
using TavernTrashers.Api.Modules.Characters.Domain.Characters;

namespace TavernTrashers.Api.Modules.Characters.Application.Characters;

public sealed record RecordDeathSavingThrowSuccessCommand(Guid CharacterId) : ICommand<DeathSavingThrowsResponse>;

internal sealed class RecordDeathSavingThrowSuccessCommandValidator : AbstractValidator<RecordDeathSavingThrowSuccessCommand>
{
	public RecordDeathSavingThrowSuccessCommandValidator()
	{
		RuleFor(x => x.CharacterId).NotEmpty();
	}
}

internal sealed class RecordDeathSavingThrowSuccessCommandHandler(
	ICharacterRepository characterRepository,
	IHubService hubService,
	IClaimsProvider claimsProvider)
	: ICommandHandler<RecordDeathSavingThrowSuccessCommand, DeathSavingThrowsResponse>
{
	public async Task<Result<DeathSavingThrowsResponse>> Handle(
		RecordDeathSavingThrowSuccessCommand command,
		CancellationToken cancellationToken)
	{
		var characterResult = await characterRepository.GetAsync(command.CharacterId, cancellationToken);
		if (characterResult.IsFailure) return characterResult.Error;

		var character = characterResult.Value;
		var oldSuccesses = character.DeathSavingThrows.Successes;

		var result = character.RecordDeathSavingThrowSuccess();
		if (result.IsFailure) return result.Error;

		await hubService.PublishAsync(
			$"campaign:{character.CampaignId}",
			"ResourceChanged",
			new ResourceChangedNotification(
				character.Id,
				character.Name,
				character.CampaignId,
				"Death Saving Throws",
				$"S:{oldSuccesses}/F:{character.DeathSavingThrows.Failures}",
				$"S:{character.DeathSavingThrows.Successes}/F:{character.DeathSavingThrows.Failures}",
				claimsProvider.GetEmail()),
			cancellationToken);

		return (DeathSavingThrowsResponse)character.DeathSavingThrows;
	}
}
