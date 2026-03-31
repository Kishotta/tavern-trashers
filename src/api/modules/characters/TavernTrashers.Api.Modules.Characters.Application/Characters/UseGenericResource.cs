using FluentValidation;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Characters.Domain.Characters;
using TavernTrashers.Api.Modules.Characters.Domain.Resources;

namespace TavernTrashers.Api.Modules.Characters.Application.Characters;

public sealed record UseGenericResourceCommand(Guid CharacterId, Guid ResourceId) : ICommand;

internal sealed class UseGenericResourceCommandValidator : AbstractValidator<UseGenericResourceCommand>
{
	public UseGenericResourceCommandValidator()
	{
		RuleFor(x => x.CharacterId).NotEmpty();
		RuleFor(x => x.ResourceId).NotEmpty();
	}
}

internal sealed class UseGenericResourceCommandHandler(ICharacterRepository characterRepository)
	: ICommandHandler<UseGenericResourceCommand>
{
	public async Task<Result> Handle(UseGenericResourceCommand command, CancellationToken cancellationToken)
	{
		var characterResult = await characterRepository.GetAsync(command.CharacterId, cancellationToken);
		if (characterResult.IsFailure) return characterResult.Error;

		return characterResult.Value.UseGenericResource(command.ResourceId);
	}
}
