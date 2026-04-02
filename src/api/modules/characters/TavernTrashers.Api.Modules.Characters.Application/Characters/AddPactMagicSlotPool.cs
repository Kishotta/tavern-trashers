using FluentValidation;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Characters.Domain.Characters;
using TavernTrashers.Api.Modules.Characters.Domain.Resources;

namespace TavernTrashers.Api.Modules.Characters.Application.Characters;

public sealed record AddPactMagicSlotPoolCommand(Guid CharacterId) : ICommand<SpellSlotPoolResponse>;

internal sealed class AddPactMagicSlotPoolCommandValidator : AbstractValidator<AddPactMagicSlotPoolCommand>
{
	public AddPactMagicSlotPoolCommandValidator()
	{
		RuleFor(x => x.CharacterId).NotEmpty();
	}
}

internal sealed class AddPactMagicSlotPoolCommandHandler(ICharacterRepository characterRepository)
	: ICommandHandler<AddPactMagicSlotPoolCommand, SpellSlotPoolResponse>
{
	public async Task<Result<SpellSlotPoolResponse>> Handle(
		AddPactMagicSlotPoolCommand command,
		CancellationToken cancellationToken)
	{
		var characterResult = await characterRepository.GetAsync(command.CharacterId, cancellationToken);
		if (characterResult.IsFailure) return characterResult.Error;

		var result = characterResult.Value.AddPactMagicSlotPool();
		if (result.IsFailure) return result.Error;

		var pool = characterResult.Value.SpellSlotPools
		   .Single(p => p.Kind == SpellSlotPoolKind.PactMagic);

		return (SpellSlotPoolResponse)pool;
	}
}
