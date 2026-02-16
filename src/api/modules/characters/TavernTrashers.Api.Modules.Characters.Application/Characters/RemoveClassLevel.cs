using FluentValidation;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Characters.Domain.Characters;

namespace TavernTrashers.Api.Modules.Characters.Application.Characters;

public sealed record RemoveClassLevelCommand(Guid CharacterId, Guid CharacterClassId) : ICommand;

internal sealed class RemoveClassLevelCommandValidator : AbstractValidator<RemoveClassLevelCommand>
{
	public RemoveClassLevelCommandValidator()
	{
		RuleFor(x => x.CharacterId).NotEmpty();
		RuleFor(x => x.CharacterClassId).NotEmpty();
	}
}

internal sealed class RemoveClassLevelCommandHandler(ICharacterRepository characterRepository)
	: ICommandHandler<RemoveClassLevelCommand>
{
	public async Task<Result> Handle(RemoveClassLevelCommand command, CancellationToken cancellationToken) =>
		await characterRepository
		   .GetAsync(command.CharacterId, cancellationToken)
		   .ThenAsync(character => character.RemoveClassLevel(command.CharacterClassId));
}
