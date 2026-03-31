using FluentValidation;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Characters.Domain.Characters;

namespace TavernTrashers.Api.Modules.Characters.Application.Characters;

public sealed record SetTemporaryHpCommand(Guid CharacterId, int Amount) : ICommand<HpTrackerResponse>;

internal sealed class SetTemporaryHpCommandValidator : AbstractValidator<SetTemporaryHpCommand>
{
	public SetTemporaryHpCommandValidator()
	{
		RuleFor(x => x.CharacterId).NotEmpty();
		RuleFor(x => x.Amount).GreaterThanOrEqualTo(0);
	}
}

internal sealed class SetTemporaryHpCommandHandler(ICharacterRepository characterRepository)
	: ICommandHandler<SetTemporaryHpCommand, HpTrackerResponse>
{
	public async Task<Result<HpTrackerResponse>> Handle(SetTemporaryHpCommand command, CancellationToken cancellationToken)
	{
		var characterResult = await characterRepository.GetAsync(command.CharacterId, cancellationToken);
		if (characterResult.IsFailure) return characterResult.Error;

		var result = characterResult.Value.SetTemporaryHp(command.Amount);
		if (result.IsFailure) return result.Error;

		return (HpTrackerResponse)characterResult.Value.HpTracker!;
	}
}
