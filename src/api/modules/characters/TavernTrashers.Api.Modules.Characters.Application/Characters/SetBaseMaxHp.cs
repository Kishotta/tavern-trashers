using FluentValidation;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Characters.Domain.Characters;

namespace TavernTrashers.Api.Modules.Characters.Application.Characters;

public sealed record SetBaseMaxHpCommand(Guid CharacterId, int BaseMaxHp) : ICommand<HpTrackerResponse>;

internal sealed class SetBaseMaxHpCommandValidator : AbstractValidator<SetBaseMaxHpCommand>
{
	public SetBaseMaxHpCommandValidator()
	{
		RuleFor(x => x.CharacterId).NotEmpty();
		RuleFor(x => x.BaseMaxHp).GreaterThan(0);
	}
}

internal sealed class SetBaseMaxHpCommandHandler(ICharacterRepository characterRepository)
	: ICommandHandler<SetBaseMaxHpCommand, HpTrackerResponse>
{
	public async Task<Result<HpTrackerResponse>> Handle(SetBaseMaxHpCommand command, CancellationToken cancellationToken)
	{
		var characterResult = await characterRepository.GetAsync(command.CharacterId, cancellationToken);
		if (characterResult.IsFailure) return characterResult.Error;

		var result = characterResult.Value.SetBaseMaxHp(command.BaseMaxHp);
		if (result.IsFailure) return result.Error;

		return (HpTrackerResponse)characterResult.Value.HpTracker!;
	}
}
