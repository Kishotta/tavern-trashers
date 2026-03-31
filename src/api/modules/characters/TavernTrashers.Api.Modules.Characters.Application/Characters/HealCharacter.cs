using FluentValidation;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Characters.Domain.Characters;

namespace TavernTrashers.Api.Modules.Characters.Application.Characters;

public sealed record HealCharacterCommand(Guid CharacterId, int Amount) : ICommand<HpTrackerResponse>;

internal sealed class HealCharacterCommandValidator : AbstractValidator<HealCharacterCommand>
{
	public HealCharacterCommandValidator()
	{
		RuleFor(x => x.CharacterId).NotEmpty();
		RuleFor(x => x.Amount).GreaterThan(0);
	}
}

internal sealed class HealCharacterCommandHandler(ICharacterRepository characterRepository)
	: ICommandHandler<HealCharacterCommand, HpTrackerResponse>
{
	public async Task<Result<HpTrackerResponse>> Handle(HealCharacterCommand command, CancellationToken cancellationToken)
	{
		var characterResult = await characterRepository.GetAsync(command.CharacterId, cancellationToken);
		if (characterResult.IsFailure) return characterResult.Error;

		var result = characterResult.Value.Heal(command.Amount);
		if (result.IsFailure) return result.Error;

		return (HpTrackerResponse)characterResult.Value.HpTracker!;
	}
}
