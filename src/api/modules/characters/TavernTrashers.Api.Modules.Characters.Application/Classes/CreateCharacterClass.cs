using FluentValidation;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Characters.Domain.Classes;

namespace TavernTrashers.Api.Modules.Characters.Application.Classes;

public sealed record CreateCharacterClassCommand(
	string Name,
	List<ResourceDefinitionRequest> ResourceDefinitions) : ICommand<CharacterClassResponse>;

public sealed record ResourceDefinitionRequest(
	string Name,
	List<ResourceAllowanceRequest> Allowances);

public sealed record ResourceAllowanceRequest(int Level, int Amount);

internal sealed class CreateCharacterClassCommandValidator : AbstractValidator<CreateCharacterClassCommand>
{
	public CreateCharacterClassCommandValidator()
	{
		RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
		RuleForEach(x => x.ResourceDefinitions).ChildRules(rd =>
		{
			rd.RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
			rd.RuleForEach(x => x.Allowances).ChildRules(a =>
			{
				a.RuleFor(x => x.Level).InclusiveBetween(1, 20);
				a.RuleFor(x => x.Amount).GreaterThanOrEqualTo(0);
			});
		});
	}
}

internal sealed class CreateCharacterClassCommandHandler(ICharacterClassRepository characterClassRepository)
	: ICommandHandler<CreateCharacterClassCommand, CharacterClassResponse>
{
	public async Task<Result<CharacterClassResponse>> Handle(
		CreateCharacterClassCommand command,
		CancellationToken cancellationToken)
	{
		if (await characterClassRepository.ExistsAsync(command.Name, cancellationToken))
			return CharacterClassErrors.DuplicateName(command.Name);

		var classResult = CharacterClass.Create(command.Name, isHomebrew: true);
		if (classResult.IsFailure) return classResult.Error;

		var characterClass = classResult.Value;

		foreach (var rdRequest in command.ResourceDefinitions)
		{
			var rdResult = characterClass.AddResourceDefinition(rdRequest.Name);
			if (rdResult.IsFailure) return rdResult.Error;

			foreach (var allowance in rdRequest.Allowances)
				rdResult.Value.AddAllowance(allowance.Level, allowance.Amount);
		}

		characterClassRepository.Add(characterClass);

		return (CharacterClassResponse)characterClass;
	}
}
