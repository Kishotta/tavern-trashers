using FluentValidation;
using TavernTrashers.Api.Common.Application.Authentication;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Characters.Domain.Characters;

namespace TavernTrashers.Api.Modules.Characters.Application.Characters;

public sealed record RemoveConditionCommand(Guid CharacterId, Conditions Condition) : ICommand<CharacterResponse>;

internal sealed class RemoveConditionCommandValidator : AbstractValidator<RemoveConditionCommand>
{
	public RemoveConditionCommandValidator()
	{
		RuleFor(x => x.CharacterId).NotEmpty();
		RuleFor(x => x.Condition).IsInEnum();
	}
}

internal sealed class RemoveConditionCommandHandler(
	ICharacterRepository characterRepository,
	IClaimsProvider claimsProvider)
	: ICommandHandler<RemoveConditionCommand, CharacterResponse>
{
	public async Task<Result<CharacterResponse>> Handle(RemoveConditionCommand command, CancellationToken cancellationToken)
	{
		var characterResult = await characterRepository.GetAsync(command.CharacterId, cancellationToken);
		if (characterResult.IsFailure) return characterResult.Error;

		characterResult.Value.RemoveCondition(command.Condition, claimsProvider.GetEmail());

		return (CharacterResponse)characterResult.Value;
	}
}
