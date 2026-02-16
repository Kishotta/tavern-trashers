using FluentValidation;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Characters.Domain.Characters;
using TavernTrashers.Api.Modules.Characters.Domain.Resources;

namespace TavernTrashers.Api.Modules.Characters.Application.Characters;

public sealed record SpendResourceCommand(Guid CharacterId, Guid ResourceId, int Amount) : ICommand;

internal sealed class SpendResourceCommandValidator : AbstractValidator<SpendResourceCommand>
{
	public SpendResourceCommandValidator()
	{
		RuleFor(x => x.CharacterId).NotEmpty();
		RuleFor(x => x.ResourceId).NotEmpty();
		RuleFor(x => x.Amount).GreaterThan(0);
	}
}

internal sealed class SpendResourceCommandHandler(ICharacterRepository characterRepository)
	: ICommandHandler<SpendResourceCommand>
{
	public async Task<Result> Handle(SpendResourceCommand command, CancellationToken cancellationToken)
	{
		var characterResult = await characterRepository.GetAsync(command.CharacterId, cancellationToken);
		if (characterResult.IsFailure) return characterResult.Error;

		var resource = characterResult.Value.Resources
		   .SingleOrDefault(r => r.Id == command.ResourceId);

		if (resource is null)
			return CharacterResourceErrors.NotFound(command.ResourceId);

		return resource.Spend(command.Amount);
	}
}
