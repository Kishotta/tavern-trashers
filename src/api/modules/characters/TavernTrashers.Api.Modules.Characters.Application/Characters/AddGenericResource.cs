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
	int MaxUses,
	ResourceDirection Direction,
	SourceCategory SourceCategory,
	IReadOnlyCollection<ResetTrigger> ResetTriggers) : ICommand<GenericResourceResponse>;

internal sealed class AddGenericResourceCommandValidator : AbstractValidator<AddGenericResourceCommand>
{
	public AddGenericResourceCommandValidator()
	{
		RuleFor(x => x.CharacterId).NotEmpty();
		RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
		RuleFor(x => x.MaxUses).GreaterThanOrEqualTo(0);
		RuleFor(x => x.ResetTriggers).NotEmpty();
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

		return characterResult.Value
		   .AddGenericResource(
				command.Name,
				command.MaxUses,
				command.Direction,
				command.SourceCategory,
				command.ResetTriggers)
		   .Transform(resource => (GenericResourceResponse)resource);
	}
}
