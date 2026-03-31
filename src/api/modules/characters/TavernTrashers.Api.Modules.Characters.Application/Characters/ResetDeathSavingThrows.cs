using FluentValidation;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Characters.Domain.Characters;

namespace TavernTrashers.Api.Modules.Characters.Application.Characters;

public sealed record ResetDeathSavingThrowsCommand(Guid CharacterId) : ICommand<DeathSavingThrowsResponse>;

internal sealed class ResetDeathSavingThrowsCommandValidator : AbstractValidator<ResetDeathSavingThrowsCommand>
{
	public ResetDeathSavingThrowsCommandValidator()
	{
		RuleFor(x => x.CharacterId).NotEmpty();
	}
}

internal sealed class ResetDeathSavingThrowsCommandHandler(ICharacterRepository characterRepository)
	: ICommandHandler<ResetDeathSavingThrowsCommand, DeathSavingThrowsResponse>
{
	public async Task<Result<DeathSavingThrowsResponse>> Handle(
		ResetDeathSavingThrowsCommand command,
		CancellationToken cancellationToken)
	{
		var characterResult = await characterRepository.GetAsync(command.CharacterId, cancellationToken);
		if (characterResult.IsFailure) return characterResult.Error;

		characterResult.Value.ResetDeathSavingThrows();

		return (DeathSavingThrowsResponse)characterResult.Value.DeathSavingThrows;
	}
}
