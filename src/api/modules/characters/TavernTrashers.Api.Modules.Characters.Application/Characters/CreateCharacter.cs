using FluentValidation;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Characters.Domain.Characters;

namespace TavernTrashers.Api.Modules.Characters.Application.Characters;

public sealed record CreateCharacterCommand(string Name, int Level, Guid OwnerId, Guid CampaignId) : ICommand<CharacterResponse>;

internal sealed class CreateCharacterCommandValidator : AbstractValidator<CreateCharacterCommand>
{
	public CreateCharacterCommandValidator()
	{
		RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
		RuleFor(x => x.Level).InclusiveBetween(1, 20);
		RuleFor(x => x.CampaignId).NotEmpty();
	}
}

internal sealed class CreateCharacterCommandHandler(ICharacterRepository characterRepository)
	: ICommandHandler<CreateCharacterCommand, CharacterResponse>
{
	public async Task<Result<CharacterResponse>> Handle(
		CreateCharacterCommand command,
		CancellationToken cancellationToken) =>
		await Task.FromResult(
			Character.Create(command.Name, command.Level, command.OwnerId, command.CampaignId)
			   .Do(characterRepository.Add)
			   .Transform(character => (CharacterResponse)character));
}
