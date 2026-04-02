using FluentValidation;
using TavernTrashers.Api.Common.Application.Authentication;
using TavernTrashers.Api.Common.Application.Hubs;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Characters.Application.Hubs;
using TavernTrashers.Api.Modules.Characters.Domain.Characters;

namespace TavernTrashers.Api.Modules.Characters.Application.Characters;

public sealed record SetTemporaryHitPointsCommand(Guid CharacterId, int Amount) : ICommand<HitPointsResponse>;

internal sealed class SetTemporaryHitPointsCommandValidator : AbstractValidator<SetTemporaryHitPointsCommand>
{
	public SetTemporaryHitPointsCommandValidator()
	{
		RuleFor(x => x.CharacterId).NotEmpty();
		RuleFor(x => x.Amount).GreaterThanOrEqualTo(0);
	}
}

internal sealed class SetTemporaryHitPointsCommandHandler(
	ICharacterRepository characterRepository,
	IHubService hubService,
	IClaimsProvider claimsProvider)
	: ICommandHandler<SetTemporaryHitPointsCommand, HitPointsResponse>
{
	public async Task<Result<HitPointsResponse>> Handle(SetTemporaryHitPointsCommand command, CancellationToken cancellationToken)
	{
		var characterResult = await characterRepository.GetAsync(command.CharacterId, cancellationToken);
		if (characterResult.IsFailure) return characterResult.Error;

		var character = characterResult.Value;
		var oldTempHp = character.HitPoints.TemporaryHitPoints;

		var result = character.SetTemporaryHitPoints(command.Amount);
		if (result.IsFailure) return result.Error;

		await hubService.PublishAsync(
			$"campaign:{character.CampaignId}",
			"ResourceChanged",
			new ResourceChangedNotification(
				character.Id,
				character.Name,
				character.CampaignId,
				"Temporary Hit Points",
				oldTempHp.ToString(),
				character.HitPoints.TemporaryHitPoints.ToString(),
				claimsProvider.GetEmail()),
			cancellationToken);

		return (HitPointsResponse)character.HitPoints;
	}
}
