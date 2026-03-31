using FluentValidation;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Characters.Domain.Characters;

namespace TavernTrashers.Api.Modules.Characters.Application.Characters;

public sealed record SetHpFieldsCommand(
	Guid CharacterId,
	int? BaseMaxHp,
	int? CurrentHp,
	int? TemporaryHp,
	int? MaxHpReduction) : ICommand<HpTrackerResponse>;

internal sealed class SetHpFieldsCommandValidator : AbstractValidator<SetHpFieldsCommand>
{
	public SetHpFieldsCommandValidator()
	{
		RuleFor(x => x.CharacterId).NotEmpty();
		RuleFor(x => x.BaseMaxHp).GreaterThan(0).When(x => x.BaseMaxHp.HasValue);
		RuleFor(x => x.CurrentHp).GreaterThanOrEqualTo(0).When(x => x.CurrentHp.HasValue);
		RuleFor(x => x.TemporaryHp).GreaterThanOrEqualTo(0).When(x => x.TemporaryHp.HasValue);
		RuleFor(x => x.MaxHpReduction).GreaterThanOrEqualTo(0).When(x => x.MaxHpReduction.HasValue);
	}
}

internal sealed class SetHpFieldsCommandHandler(ICharacterRepository characterRepository)
	: ICommandHandler<SetHpFieldsCommand, HpTrackerResponse>
{
	public async Task<Result<HpTrackerResponse>> Handle(SetHpFieldsCommand command, CancellationToken cancellationToken)
	{
		var characterResult = await characterRepository.GetAsync(command.CharacterId, cancellationToken);
		if (characterResult.IsFailure) return characterResult.Error;

		var result = characterResult.Value.SetHpFields(
			command.BaseMaxHp,
			command.CurrentHp,
			command.TemporaryHp,
			command.MaxHpReduction);

		if (result.IsFailure) return result.Error;

		return (HpTrackerResponse)characterResult.Value.HpTracker!;
	}
}
