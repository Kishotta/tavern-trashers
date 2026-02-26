using FluentValidation;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Characters.Domain.Classes;

namespace TavernTrashers.Api.Modules.Characters.Application.Classes;

public sealed record CreateClassCommand(string Name) : ICommand<ClassResponse>;

internal sealed class CreateClassCommandValidator : AbstractValidator<CreateClassCommand>
{
	public CreateClassCommandValidator()
	{
		RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
	}
}

internal sealed class CreateClassCommandHandler(IClassRepository classRepository)
	: ICommandHandler<CreateClassCommand, ClassResponse>
{
	public async Task<Result<ClassResponse>> Handle(
		CreateClassCommand command,
		CancellationToken cancellationToken)
	{
		if (await classRepository.ExistsAsync(command.Name, cancellationToken))
			return ClassErrors.DuplicateName(command.Name);

		return await Task.FromResult(
			Class.Create(command.Name)
			   .Do(classRepository.Add)
			   .Transform(c => (ClassResponse)c));
	}
}
