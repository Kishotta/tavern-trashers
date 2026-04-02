using FluentValidation;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Characters.Domain.Characters;

namespace TavernTrashers.Api.Modules.Characters.Application.Characters;

public sealed record RemoveMaxHitPointReductionCommand(Guid CharacterId) : ICommand<HitPointsResponse>;

internal sealed class RemoveMaxHitPointReductionCommandValidator : AbstractValidator<RemoveMaxHitPointReductionCommand>
{
	public RemoveMaxHitPointReductionCommandValidator()
	{
		RuleFor(x => x.CharacterId).NotEmpty();
	}
}

internal sealed class RemoveMaxHitPointReductionCommandHandler(
	ICharacterRepository characterRepository)
	: ICommandHandler<RemoveMaxHitPointReductionCommand, HitPointsResponse>
{
	public async Task<Result<HitPointsResponse>> Handle(RemoveMaxHitPointReductionCommand command, CancellationToken cancellationToken)
	{
		var characterResult = await characterRepository.GetAsync(command.CharacterId, cancellationToken);
		if (characterResult.IsFailure) return characterResult.Error;

		var result = characterResult.Value.RemoveMaxHitPointReduction();
		if (result.IsFailure) return result.Error;

		return (HitPointsResponse)characterResult.Value.HitPoints;
	}
}
