using FluentValidation;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Characters.Domain.Characters;

namespace TavernTrashers.Api.Modules.Characters.Application.Characters;

public sealed record ApplyConditionCommand(Guid CharacterId, Conditions Condition) : ICommand<CharacterResponse>;

internal sealed class ApplyConditionCommandValidator : AbstractValidator<ApplyConditionCommand>
{
	public ApplyConditionCommandValidator()
	{
		RuleFor(x => x.CharacterId).NotEmpty();
		RuleFor(x => x.Condition).IsInEnum();
	}
}

internal sealed class ApplyConditionCommandHandler(
	ICharacterRepository characterRepository)
	: ICommandHandler<ApplyConditionCommand, CharacterResponse>
{
	public async Task<Result<CharacterResponse>> Handle(ApplyConditionCommand command, CancellationToken cancellationToken)
	{
		var characterResult = await characterRepository.GetAsync(command.CharacterId, cancellationToken);
		if (characterResult.IsFailure) return characterResult.Error;

		characterResult.Value.ApplyCondition(command.Condition);

		return (CharacterResponse)characterResult.Value;
	}
}
