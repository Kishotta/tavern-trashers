using FluentValidation;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Characters.Domain.Characters;

namespace TavernTrashers.Api.Modules.Characters.Application.Characters;

public sealed record UpdateCharacterCommand(Guid CharacterId, string Name) : ICommand;

internal sealed class UpdateCharacterCommandValidator : AbstractValidator<UpdateCharacterCommand>
{
	public UpdateCharacterCommandValidator()
	{
		RuleFor(x => x.CharacterId).NotEmpty();
		RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
	}
}

internal sealed class UpdateCharacterCommandHandler(ICharacterRepository characterRepository)
	: ICommandHandler<UpdateCharacterCommand>
{
	public async Task<Result> Handle(UpdateCharacterCommand command, CancellationToken cancellationToken) =>
		await characterRepository
		   .GetAsync(command.CharacterId, cancellationToken)
		   .DoAsync(character => character.ChangeName(command.Name));
}
