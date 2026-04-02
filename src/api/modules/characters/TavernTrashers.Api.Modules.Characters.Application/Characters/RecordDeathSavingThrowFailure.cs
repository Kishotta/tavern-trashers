using FluentValidation;
using TavernTrashers.Api.Common.Application.Authentication;
using TavernTrashers.Api.Common.Application.Hubs;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Characters.Application.Hubs;
using TavernTrashers.Api.Modules.Characters.Domain.Characters;

namespace TavernTrashers.Api.Modules.Characters.Application.Characters;

public sealed record RecordDeathSavingThrowFailureCommand(Guid CharacterId) : ICommand<DeathSavingThrowsResponse>;

internal sealed class RecordDeathSavingThrowFailureCommandValidator : AbstractValidator<RecordDeathSavingThrowFailureCommand>
{
	public RecordDeathSavingThrowFailureCommandValidator()
	{
		RuleFor(x => x.CharacterId).NotEmpty();
	}
}

internal sealed class RecordDeathSavingThrowFailureCommandHandler(
	ICharacterRepository characterRepository,
	IHubService hubService,
	IClaimsProvider claimsProvider)
	: ICommandHandler<RecordDeathSavingThrowFailureCommand, DeathSavingThrowsResponse>
{
	public async Task<Result<DeathSavingThrowsResponse>> Handle(
		RecordDeathSavingThrowFailureCommand command,
		CancellationToken cancellationToken)
	{
		var characterResult = await characterRepository.GetAsync(command.CharacterId, cancellationToken);
		if (characterResult.IsFailure) return characterResult.Error;

		var character = characterResult.Value;
		var oldFailures = character.DeathSavingThrows.Failures;

		var result = character.RecordDeathSavingThrowFailure();
		if (result.IsFailure) return result.Error;

		await hubService.PublishAsync(
			$"campaign:{character.CampaignId}",
			"ResourceChanged",
			new ResourceChangedNotification(
				character.Id,
				character.Name,
				character.CampaignId,
				"Death Saving Throws",
				$"S:{character.DeathSavingThrows.Successes}/F:{oldFailures}",
				$"S:{character.DeathSavingThrows.Successes}/F:{character.DeathSavingThrows.Failures}",
				claimsProvider.GetEmail()),
			cancellationToken);

		return (DeathSavingThrowsResponse)character.DeathSavingThrows;
	}
}
