using FluentValidation;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Characters.Domain.Characters;

namespace TavernTrashers.Api.Modules.Characters.Application.Characters;

public sealed record CreateCharacterCommand(string Name) : ICommand<CharacterResponse>;

internal sealed class CreateCharacterCommandValidator : AbstractValidator<CreateCharacterCommand>
{
	public CreateCharacterCommandValidator()
	{
		RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
	}
}

internal sealed class CreateCharacterCommandHandler(ICharacterRepository characterRepository)
	: ICommandHandler<CreateCharacterCommand, CharacterResponse>
{
	public async Task<Result<CharacterResponse>> Handle(
		CreateCharacterCommand command,
		CancellationToken cancellationToken) =>
		await Task.FromResult(
			Character.Create(command.Name)
			   .Do(characterRepository.Add)
			   .Transform(character => (CharacterResponse)character));
}
