using FluentValidation;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Characters.Domain.Characters;

namespace TavernTrashers.Api.Modules.Characters.Application.Characters;

public sealed record ApplyMaxHpReductionCommand(Guid CharacterId, int Reduction) : ICommand<HpTrackerResponse>;

internal sealed class ApplyMaxHpReductionCommandValidator : AbstractValidator<ApplyMaxHpReductionCommand>
{
	public ApplyMaxHpReductionCommandValidator()
	{
		RuleFor(x => x.CharacterId).NotEmpty();
		RuleFor(x => x.Reduction).GreaterThan(0);
	}
}

internal sealed class ApplyMaxHpReductionCommandHandler(ICharacterRepository characterRepository)
	: ICommandHandler<ApplyMaxHpReductionCommand, HpTrackerResponse>
{
	public async Task<Result<HpTrackerResponse>> Handle(ApplyMaxHpReductionCommand command, CancellationToken cancellationToken)
	{
		var characterResult = await characterRepository.GetAsync(command.CharacterId, cancellationToken);
		if (characterResult.IsFailure) return characterResult.Error;

		var result = characterResult.Value.ApplyMaxHpReduction(command.Reduction);
		if (result.IsFailure) return result.Error;

		return (HpTrackerResponse)characterResult.Value.HpTracker!;
	}
}
