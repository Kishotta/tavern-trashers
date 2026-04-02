using FluentValidation;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Characters.Domain.Characters;

namespace TavernTrashers.Api.Modules.Characters.Application.Characters;

public sealed record SetTemporaryHitPointsCommand(Guid CharacterId, int Amount) : ICommand<HitPointsResponse>;

internal sealed class SetTemporaryHitPointsCommandValidator : AbstractValidator<SetTemporaryHitPointsCommand>
{
	public SetTemporaryHitPointsCommandValidator()
	{
		RuleFor(x => x.CharacterId).NotEmpty();
		RuleFor(x => x.Amount).GreaterThanOrEqualTo(0);
	}
}

internal sealed class SetTemporaryHitPointsCommandHandler(
	ICharacterRepository characterRepository)
	: ICommandHandler<SetTemporaryHitPointsCommand, HitPointsResponse>
{
	public async Task<Result<HitPointsResponse>> Handle(SetTemporaryHitPointsCommand command, CancellationToken cancellationToken)
	{
		var characterResult = await characterRepository.GetAsync(command.CharacterId, cancellationToken);
		if (characterResult.IsFailure) return characterResult.Error;

		var result = characterResult.Value.SetTemporaryHitPoints(command.Amount);
		if (result.IsFailure) return result.Error;

		return (HitPointsResponse)characterResult.Value.HitPoints;
	}
}
