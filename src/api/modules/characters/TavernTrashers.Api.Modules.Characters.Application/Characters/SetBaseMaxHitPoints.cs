using FluentValidation;
using TavernTrashers.Api.Common.Application.Authentication;
using TavernTrashers.Api.Common.Application.Hubs;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Characters.Application.Hubs;
using TavernTrashers.Api.Modules.Characters.Domain.Characters;

namespace TavernTrashers.Api.Modules.Characters.Application.Characters;

public sealed record SetBaseMaxHitPointsCommand(Guid CharacterId, int BaseMaxHitPoints) : ICommand<HitPointsResponse>;

internal sealed class SetBaseMaxHitPointsCommandValidator : AbstractValidator<SetBaseMaxHitPointsCommand>
{
	public SetBaseMaxHitPointsCommandValidator()
	{
		RuleFor(x => x.CharacterId).NotEmpty();
		RuleFor(x => x.BaseMaxHitPoints).GreaterThanOrEqualTo(0);
	}
}

internal sealed class SetBaseMaxHitPointsCommandHandler(
	ICharacterRepository characterRepository,
	IHubService hubService,
	IClaimsProvider claimsProvider)
	: ICommandHandler<SetBaseMaxHitPointsCommand, HitPointsResponse>
{
	public async Task<Result<HitPointsResponse>> Handle(SetBaseMaxHitPointsCommand command, CancellationToken cancellationToken)
	{
		var characterResult = await characterRepository.GetAsync(command.CharacterId, cancellationToken);
		if (characterResult.IsFailure) return characterResult.Error;

		var character = characterResult.Value;
		var oldMax = character.HitPoints.BaseMaxHitPoints;

		var result = character.SetBaseMaxHitPoints(command.BaseMaxHitPoints);
		if (result.IsFailure) return result.Error;

		await hubService.PublishAsync(
			$"campaign:{character.CampaignId}",
			"ResourceChanged",
			new ResourceChangedNotification(
				character.Id,
				character.Name,
				character.CampaignId,
				"Max Hit Points",
				oldMax.ToString(),
				character.HitPoints.BaseMaxHitPoints.ToString(),
				claimsProvider.GetEmail()),
			cancellationToken);

		return (HitPointsResponse)character.HitPoints;
	}
}
