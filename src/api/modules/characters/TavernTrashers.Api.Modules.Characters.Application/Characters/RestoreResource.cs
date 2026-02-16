using FluentValidation;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Characters.Domain.Characters;
using TavernTrashers.Api.Modules.Characters.Domain.Resources;

namespace TavernTrashers.Api.Modules.Characters.Application.Characters;

public sealed record RestoreResourceCommand(Guid CharacterId, Guid ResourceId, int Amount) : ICommand;

internal sealed class RestoreResourceCommandValidator : AbstractValidator<RestoreResourceCommand>
{
	public RestoreResourceCommandValidator()
	{
		RuleFor(x => x.CharacterId).NotEmpty();
		RuleFor(x => x.ResourceId).NotEmpty();
		RuleFor(x => x.Amount).GreaterThan(0);
	}
}

internal sealed class RestoreResourceCommandHandler(ICharacterRepository characterRepository)
	: ICommandHandler<RestoreResourceCommand>
{
	public async Task<Result> Handle(RestoreResourceCommand command, CancellationToken cancellationToken)
	{
		var characterResult = await characterRepository.GetAsync(command.CharacterId, cancellationToken);
		if (characterResult.IsFailure) return characterResult.Error;

		var resource = characterResult.Value.Resources
		   .SingleOrDefault(r => r.Id == command.ResourceId);

		if (resource is null)
			return CharacterResourceErrors.NotFound(command.ResourceId);

		return resource.Restore(command.Amount);
	}
}
