using FluentValidation;
using TavernTrashers.Api.Common.Application.Authentication;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Characters.Domain.Characters;

namespace TavernTrashers.Api.Modules.Characters.Application.Characters;

public sealed record RecordDeathSavingThrowSuccessCommand(Guid CharacterId) : ICommand<DeathSavingThrowsResponse>;

internal sealed class RecordDeathSavingThrowSuccessCommandValidator : AbstractValidator<RecordDeathSavingThrowSuccessCommand>
{
	public RecordDeathSavingThrowSuccessCommandValidator()
	{
		RuleFor(x => x.CharacterId).NotEmpty();
	}
}

internal sealed class RecordDeathSavingThrowSuccessCommandHandler(
	ICharacterRepository characterRepository,
	IClaimsProvider claimsProvider)
	: ICommandHandler<RecordDeathSavingThrowSuccessCommand, DeathSavingThrowsResponse>
{
	public async Task<Result<DeathSavingThrowsResponse>> Handle(
		RecordDeathSavingThrowSuccessCommand command,
		CancellationToken cancellationToken)
	{
		var characterResult = await characterRepository.GetAsync(command.CharacterId, cancellationToken);
		if (characterResult.IsFailure) return characterResult.Error;

		var result = characterResult.Value.RecordDeathSavingThrowSuccess(claimsProvider.GetEmail());
		if (result.IsFailure) return result.Error;

		return (DeathSavingThrowsResponse)characterResult.Value.DeathSavingThrows;
	}
}
