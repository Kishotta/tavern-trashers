using FluentValidation;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Characters.Domain.Characters;

namespace TavernTrashers.Api.Modules.Characters.Application.Characters;

public sealed record SetMaxSpellSlotsCommand(Guid CharacterId, Guid PoolId, int Level, int Max) : ICommand<SpellSlotPoolResponse>;

internal sealed class SetMaxSpellSlotsCommandValidator : AbstractValidator<SetMaxSpellSlotsCommand>
{
	public SetMaxSpellSlotsCommandValidator()
	{
		RuleFor(x => x.CharacterId).NotEmpty();
		RuleFor(x => x.PoolId).NotEmpty();
		RuleFor(x => x.Level).InclusiveBetween(1, 9);
		RuleFor(x => x.Max).GreaterThanOrEqualTo(0);
	}
}

internal sealed class SetMaxSpellSlotsCommandHandler(ICharacterRepository characterRepository)
	: ICommandHandler<SetMaxSpellSlotsCommand, SpellSlotPoolResponse>
{
	public async Task<Result<SpellSlotPoolResponse>> Handle(
		SetMaxSpellSlotsCommand command,
		CancellationToken cancellationToken)
	{
		var characterResult = await characterRepository.GetAsync(command.CharacterId, cancellationToken);
		if (characterResult.IsFailure) return characterResult.Error;

		var result = characterResult.Value.SetMaxSpellSlots(command.PoolId, command.Level, command.Max);
		if (result.IsFailure) return result.Error;

		var pool = characterResult.Value.SpellSlotPools.Single(p => p.Id == command.PoolId);
		return (SpellSlotPoolResponse)pool;
	}
}
