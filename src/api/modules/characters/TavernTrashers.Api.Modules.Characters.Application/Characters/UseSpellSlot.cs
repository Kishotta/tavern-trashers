using FluentValidation;
using TavernTrashers.Api.Common.Application.Authentication;
using TavernTrashers.Api.Common.Application.Hubs;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Modules.Characters.Application.Hubs;
using TavernTrashers.Api.Modules.Characters.Domain.Characters;

namespace TavernTrashers.Api.Modules.Characters.Application.Characters;

public sealed record UseSpellSlotCommand(Guid CharacterId, Guid PoolId, int Level) : ICommand;

internal sealed class UseSpellSlotCommandValidator : AbstractValidator<UseSpellSlotCommand>
{
	public UseSpellSlotCommandValidator()
	{
		RuleFor(x => x.CharacterId).NotEmpty();
		RuleFor(x => x.PoolId).NotEmpty();
		RuleFor(x => x.Level).InclusiveBetween(1, 9);
	}
}

internal sealed class UseSpellSlotCommandHandler(
	ICharacterRepository characterRepository,
	IHubService hubService,
	IClaimsProvider claimsProvider)
	: ICommandHandler<UseSpellSlotCommand>
{
	public async Task<Result> Handle(UseSpellSlotCommand command, CancellationToken cancellationToken)
	{
		var characterResult = await characterRepository.GetAsync(command.CharacterId, cancellationToken);
		if (characterResult.IsFailure) return characterResult.Error;

		var character = characterResult.Value;
		var pool = character.SpellSlotPools.SingleOrDefault(p => p.Id == command.PoolId);
		var slot = pool?.Levels.SingleOrDefault(l => l.Level == command.Level);
		var oldUses = slot?.CurrentUses ?? 0;
		var maxUses = slot?.MaxUses ?? 0;

		var result = character.UseSpellSlot(command.PoolId, command.Level);
		if (result.IsFailure) return result.Error;

		var updatedSlot = character.SpellSlotPools
			.SingleOrDefault(p => p.Id == command.PoolId)
			?.Levels.SingleOrDefault(l => l.Level == command.Level);

		await hubService.PublishAsync(
			$"campaign:{character.CampaignId}",
			"ResourceChanged",
			new ResourceChangedNotification(
				character.Id,
				character.Name,
				character.CampaignId,
				$"Level {command.Level} Spell Slots",
				$"{oldUses}/{maxUses}",
				$"{updatedSlot?.CurrentUses ?? 0}/{maxUses}",
				claimsProvider.GetEmail()),
			cancellationToken);

		return Result.Success();
	}
}
