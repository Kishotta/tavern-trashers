using FluentValidation;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Characters.Domain.Characters;
using TavernTrashers.Api.Modules.Characters.Domain.Resources;

namespace TavernTrashers.Api.Modules.Characters.Application.Characters;

public sealed record AddGenericResourceCommand(
	Guid CharacterId,
	string Name,
	int MaxAmount,
	ResourceDirection Direction,
	ResetTrigger ResetTriggers,
	string? SourceCategory = null) : ICommand<GenericResourceResponse>;

internal sealed class AddGenericResourceCommandValidator : AbstractValidator<AddGenericResourceCommand>
{
	public AddGenericResourceCommandValidator()
	{
		RuleFor(x => x.CharacterId).NotEmpty();
		RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
		RuleFor(x => x.MaxAmount).GreaterThanOrEqualTo(0);
	}
}

internal sealed class AddGenericResourceCommandHandler(ICharacterRepository characterRepository)
	: ICommandHandler<AddGenericResourceCommand, GenericResourceResponse>
{
	public async Task<Result<GenericResourceResponse>> Handle(
		AddGenericResourceCommand command,
		CancellationToken cancellationToken)
	{
		var characterResult = await characterRepository.GetAsync(command.CharacterId, cancellationToken);
		if (characterResult.IsFailure) return characterResult.Error;

		var character = characterResult.Value;
		var result    = character.AddGenericResource(command.Name, command.MaxAmount, command.Direction, command.ResetTriggers, command.SourceCategory);
		if (result.IsFailure) return result.Error;

		var resource = character.GenericResources.Last();
		return (GenericResourceResponse)resource;
	}
}
