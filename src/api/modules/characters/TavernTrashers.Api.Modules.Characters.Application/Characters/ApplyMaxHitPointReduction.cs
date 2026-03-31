using FluentValidation;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Characters.Domain.Characters;

namespace TavernTrashers.Api.Modules.Characters.Application.Characters;

public sealed record ApplyMaxHitPointReductionCommand(Guid CharacterId, int Reduction) : ICommand<HitPointsResponse>;

internal sealed class ApplyMaxHitPointReductionCommandValidator : AbstractValidator<ApplyMaxHitPointReductionCommand>
{
	public ApplyMaxHitPointReductionCommandValidator()
	{
		RuleFor(x => x.CharacterId).NotEmpty();
		RuleFor(x => x.Reduction).GreaterThan(0);
	}
}

internal sealed class ApplyMaxHitPointReductionCommandHandler(ICharacterRepository characterRepository)
	: ICommandHandler<ApplyMaxHitPointReductionCommand, HitPointsResponse>
{
	public async Task<Result<HitPointsResponse>> Handle(ApplyMaxHitPointReductionCommand command, CancellationToken cancellationToken)
	{
		var characterResult = await characterRepository.GetAsync(command.CharacterId, cancellationToken);
		if (characterResult.IsFailure) return characterResult.Error;

		var result = characterResult.Value.ApplyMaxHitPointReduction(command.Reduction);
		if (result.IsFailure) return result.Error;

		return (HitPointsResponse)characterResult.Value.HitPoints;
	}
}
