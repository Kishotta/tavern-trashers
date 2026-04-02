using FluentValidation;
using TavernTrashers.Api.Common.Application.Authentication;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Characters.Domain.Characters;
using TavernTrashers.Api.Modules.Characters.Domain.Resources;

namespace TavernTrashers.Api.Modules.Characters.Application.Characters;

public sealed record RestoreGenericResourceCommand(Guid CharacterId, Guid ResourceId) : ICommand;

internal sealed class RestoreGenericResourceCommandValidator : AbstractValidator<RestoreGenericResourceCommand>
{
	public RestoreGenericResourceCommandValidator()
	{
		RuleFor(x => x.CharacterId).NotEmpty();
		RuleFor(x => x.ResourceId).NotEmpty();
	}
}

internal sealed class RestoreGenericResourceCommandHandler(
	ICharacterRepository characterRepository,
	IClaimsProvider claimsProvider)
	: ICommandHandler<RestoreGenericResourceCommand>
{
	public async Task<Result> Handle(RestoreGenericResourceCommand command, CancellationToken cancellationToken)
	{
		var characterResult = await characterRepository.GetAsync(command.CharacterId, cancellationToken);
		if (characterResult.IsFailure) return characterResult.Error;

		return characterResult.Value.RestoreGenericResource(command.ResourceId, claimsProvider.GetEmail());
	}
}
