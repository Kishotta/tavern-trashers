using FluentValidation;
using TavernTrashers.Api.Common.Application.Authentication;
using TavernTrashers.Api.Common.Application.Hubs;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Characters.Application.Hubs;
using TavernTrashers.Api.Modules.Characters.Domain.Characters;

namespace TavernTrashers.Api.Modules.Characters.Application.Characters;

public sealed record RemoveMaxHitPointReductionCommand(Guid CharacterId) : ICommand<HitPointsResponse>;

internal sealed class RemoveMaxHitPointReductionCommandValidator : AbstractValidator<RemoveMaxHitPointReductionCommand>
{
	public RemoveMaxHitPointReductionCommandValidator()
	{
		RuleFor(x => x.CharacterId).NotEmpty();
	}
}

internal sealed class RemoveMaxHitPointReductionCommandHandler(
	ICharacterRepository characterRepository,
	IHubService hubService,
	IClaimsProvider claimsProvider)
	: ICommandHandler<RemoveMaxHitPointReductionCommand, HitPointsResponse>
{
	public async Task<Result<HitPointsResponse>> Handle(RemoveMaxHitPointReductionCommand command, CancellationToken cancellationToken)
	{
		var characterResult = await characterRepository.GetAsync(command.CharacterId, cancellationToken);
		if (characterResult.IsFailure) return characterResult.Error;

		var character = characterResult.Value;
		var oldReduction = character.HitPoints.MaxHitPointReduction;

		var result = character.RemoveMaxHitPointReduction();
		if (result.IsFailure) return result.Error;

		await hubService.PublishAsync(
			$"campaign:{character.CampaignId}",
			"ResourceChanged",
			new ResourceChangedNotification(
				character.Id,
				character.Name,
				character.CampaignId,
				"Max HP Reduction",
				oldReduction.ToString(),
				character.HitPoints.MaxHitPointReduction.ToString(),
				claimsProvider.GetEmail()),
			cancellationToken);

		return (HitPointsResponse)character.HitPoints;
	}
}
