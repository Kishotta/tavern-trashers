using FluentValidation;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Characters.Domain.Characters;

namespace TavernTrashers.Api.Modules.Characters.Application.Characters;

public sealed record DeleteCharacterCommand(Guid CharacterId) : ICommand;

internal sealed class DeleteCharacterCommandValidator : AbstractValidator<DeleteCharacterCommand>
{
	public DeleteCharacterCommandValidator()
	{
		RuleFor(x => x.CharacterId).NotEmpty();
	}
}

internal sealed class DeleteCharacterCommandHandler(ICharacterRepository characterRepository)
	: ICommandHandler<DeleteCharacterCommand>
{
	public async Task<Result> Handle(DeleteCharacterCommand command, CancellationToken cancellationToken) =>
		await characterRepository
		   .GetAsync(command.CharacterId, cancellationToken)
		   .DoAsync(characterRepository.Remove);
}
