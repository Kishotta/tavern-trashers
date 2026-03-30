using FluentValidation;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Characters.Domain.Characters;

namespace TavernTrashers.Api.Modules.Characters.Application.Characters;

public sealed record RemoveGenericResourceCommand(Guid CharacterId, Guid ResourceId) : ICommand;

internal sealed class RemoveGenericResourceCommandValidator : AbstractValidator<RemoveGenericResourceCommand>
{
	public RemoveGenericResourceCommandValidator()
	{
		RuleFor(x => x.CharacterId).NotEmpty();
		RuleFor(x => x.ResourceId).NotEmpty();
	}
}

internal sealed class RemoveGenericResourceCommandHandler(ICharacterRepository characterRepository)
	: ICommandHandler<RemoveGenericResourceCommand>
{
	public async Task<Result> Handle(RemoveGenericResourceCommand command, CancellationToken cancellationToken)
	{
		var characterResult = await characterRepository.GetAsync(command.CharacterId, cancellationToken);
		if (characterResult.IsFailure) return characterResult.Error;

		return characterResult.Value.RemoveGenericResource(command.ResourceId);
	}
}
