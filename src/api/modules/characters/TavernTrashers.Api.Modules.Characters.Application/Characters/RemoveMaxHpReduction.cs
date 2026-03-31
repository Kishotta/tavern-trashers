using FluentValidation;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Characters.Domain.Characters;

namespace TavernTrashers.Api.Modules.Characters.Application.Characters;

public sealed record RemoveMaxHpReductionCommand(Guid CharacterId) : ICommand<HpTrackerResponse>;

internal sealed class RemoveMaxHpReductionCommandValidator : AbstractValidator<RemoveMaxHpReductionCommand>
{
	public RemoveMaxHpReductionCommandValidator()
	{
		RuleFor(x => x.CharacterId).NotEmpty();
	}
}

internal sealed class RemoveMaxHpReductionCommandHandler(ICharacterRepository characterRepository)
	: ICommandHandler<RemoveMaxHpReductionCommand, HpTrackerResponse>
{
	public async Task<Result<HpTrackerResponse>> Handle(RemoveMaxHpReductionCommand command, CancellationToken cancellationToken)
	{
		var characterResult = await characterRepository.GetAsync(command.CharacterId, cancellationToken);
		if (characterResult.IsFailure) return characterResult.Error;

		var result = characterResult.Value.RemoveMaxHpReduction();
		if (result.IsFailure) return result.Error;

		return (HpTrackerResponse)characterResult.Value.HpTracker!;
	}
}
