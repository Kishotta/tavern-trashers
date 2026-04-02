using FluentValidation;
using TavernTrashers.Api.Common.Application.Authentication;
using TavernTrashers.Api.Common.Application.Hubs;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Characters.Application.Hubs;
using TavernTrashers.Api.Modules.Characters.Domain.Characters;

namespace TavernTrashers.Api.Modules.Characters.Application.Characters;

public sealed record SetHitPointFieldsCommand(
	Guid CharacterId,
	int? BaseMaxHitPoints,
	int? CurrentHitPoints,
	int? TemporaryHitPoints,
	int? MaxHitPointReduction) : ICommand<HitPointsResponse>;

internal sealed class SetHitPointFieldsCommandValidator : AbstractValidator<SetHitPointFieldsCommand>
{
	public SetHitPointFieldsCommandValidator()
	{
		RuleFor(x => x.CharacterId).NotEmpty();
		RuleFor(x => x.BaseMaxHitPoints).GreaterThanOrEqualTo(0).When(x => x.BaseMaxHitPoints.HasValue);
		RuleFor(x => x.CurrentHitPoints).GreaterThanOrEqualTo(0).When(x => x.CurrentHitPoints.HasValue);
		RuleFor(x => x.TemporaryHitPoints).GreaterThanOrEqualTo(0).When(x => x.TemporaryHitPoints.HasValue);
		RuleFor(x => x.MaxHitPointReduction).GreaterThanOrEqualTo(0).When(x => x.MaxHitPointReduction.HasValue);
	}
}

internal sealed class SetHitPointFieldsCommandHandler(
	ICharacterRepository characterRepository,
	IHubService hubService,
	IClaimsProvider claimsProvider)
	: ICommandHandler<SetHitPointFieldsCommand, HitPointsResponse>
{
	public async Task<Result<HitPointsResponse>> Handle(SetHitPointFieldsCommand command, CancellationToken cancellationToken)
	{
		var characterResult = await characterRepository.GetAsync(command.CharacterId, cancellationToken);
		if (characterResult.IsFailure) return characterResult.Error;

		var character = characterResult.Value;
		var oldHp = character.HitPoints.CurrentHitPoints;
		var oldMax = character.HitPoints.EffectiveMaxHitPoints;

		var result = character.SetHitPointFields(
			command.BaseMaxHitPoints,
			command.CurrentHitPoints,
			command.TemporaryHitPoints,
			command.MaxHitPointReduction);

		if (result.IsFailure) return result.Error;

		await hubService.PublishAsync(
			$"campaign:{character.CampaignId}",
			"ResourceChanged",
			new ResourceChangedNotification(
				character.Id,
				character.Name,
				character.CampaignId,
				"Hit Points",
				$"{oldHp}/{oldMax}",
				$"{character.HitPoints.CurrentHitPoints}/{character.HitPoints.EffectiveMaxHitPoints}",
				claimsProvider.GetEmail()),
			cancellationToken);

		return (HitPointsResponse)character.HitPoints;
	}
}
