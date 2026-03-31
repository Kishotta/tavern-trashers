using FluentValidation;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Characters.Domain.Characters;

namespace TavernTrashers.Api.Modules.Characters.Application.Characters;

public sealed record RecordDeathSavingThrowFailureCommand(Guid CharacterId) : ICommand<DeathSavingThrowsResponse>;

internal sealed class RecordDeathSavingThrowFailureCommandValidator : AbstractValidator<RecordDeathSavingThrowFailureCommand>
{
	public RecordDeathSavingThrowFailureCommandValidator()
	{
		RuleFor(x => x.CharacterId).NotEmpty();
	}
}

internal sealed class RecordDeathSavingThrowFailureCommandHandler(ICharacterRepository characterRepository)
	: ICommandHandler<RecordDeathSavingThrowFailureCommand, DeathSavingThrowsResponse>
{
	public async Task<Result<DeathSavingThrowsResponse>> Handle(
		RecordDeathSavingThrowFailureCommand command,
		CancellationToken cancellationToken)
	{
		var characterResult = await characterRepository.GetAsync(command.CharacterId, cancellationToken);
		if (characterResult.IsFailure) return characterResult.Error;

		var result = characterResult.Value.RecordDeathSavingThrowFailure();
		if (result.IsFailure) return result.Error;

		return (DeathSavingThrowsResponse)characterResult.Value.DeathSavingThrows;
	}
}
