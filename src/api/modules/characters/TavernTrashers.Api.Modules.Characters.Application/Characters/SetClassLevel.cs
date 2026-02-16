using FluentValidation;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Characters.Domain.Characters;
using TavernTrashers.Api.Modules.Characters.Domain.Classes;

namespace TavernTrashers.Api.Modules.Characters.Application.Characters;

public sealed record SetClassLevelCommand(Guid CharacterId, Guid CharacterClassId, int Level) : ICommand;

internal sealed class SetClassLevelCommandValidator : AbstractValidator<SetClassLevelCommand>
{
	public SetClassLevelCommandValidator()
	{
		RuleFor(x => x.CharacterId).NotEmpty();
		RuleFor(x => x.CharacterClassId).NotEmpty();
		RuleFor(x => x.Level).InclusiveBetween(1, 20);
	}
}

internal sealed class SetClassLevelCommandHandler(
	ICharacterRepository characterRepository,
	ICharacterClassRepository characterClassRepository)
	: ICommandHandler<SetClassLevelCommand>
{
	public async Task<Result> Handle(SetClassLevelCommand command, CancellationToken cancellationToken)
	{
		var characterResult = await characterRepository.GetAsync(command.CharacterId, cancellationToken);
		if (characterResult.IsFailure) return characterResult.Error;

		var classResult = await characterClassRepository.GetAsync(command.CharacterClassId, cancellationToken);
		if (classResult.IsFailure) return classResult.Error;

		return characterResult.Value.SetClassLevel(classResult.Value, command.Level);
	}
}
