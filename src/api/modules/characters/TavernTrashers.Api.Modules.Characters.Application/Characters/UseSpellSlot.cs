using FluentValidation;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
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

internal sealed class UseSpellSlotCommandHandler(ICharacterRepository characterRepository)
	: ICommandHandler<UseSpellSlotCommand>
{
	public async Task<Result> Handle(UseSpellSlotCommand command, CancellationToken cancellationToken)
	{
		var characterResult = await characterRepository.GetAsync(command.CharacterId, cancellationToken);
		if (characterResult.IsFailure) return characterResult.Error;

		return characterResult.Value.UseSpellSlot(command.PoolId, command.Level);
	}
}
