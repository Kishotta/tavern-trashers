using FluentValidation;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Modules.Characters.Domain.Characters;

namespace TavernTrashers.Api.Modules.Characters.Application.Characters;

public sealed record RestoreSlotPoolCommand(Guid CharacterId, Guid PoolId) : ICommand;

internal sealed class RestoreSlotPoolCommandValidator : AbstractValidator<RestoreSlotPoolCommand>
{
	public RestoreSlotPoolCommandValidator()
	{
		RuleFor(x => x.CharacterId).NotEmpty();
		RuleFor(x => x.PoolId).NotEmpty();
	}
}

internal sealed class RestoreSlotPoolCommandHandler(ICharacterRepository characterRepository)
	: ICommandHandler<RestoreSlotPoolCommand>
{
	public async Task<Result> Handle(RestoreSlotPoolCommand command, CancellationToken cancellationToken)
	{
		var characterResult = await characterRepository.GetAsync(command.CharacterId, cancellationToken);
		if (characterResult.IsFailure) return characterResult.Error;

		return characterResult.Value.RestoreSlotPool(command.PoolId);
	}
}
