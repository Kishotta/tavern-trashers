using FluentValidation;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Characters.Domain.Classes;

namespace TavernTrashers.Api.Modules.Characters.Application.Classes;

public sealed record RenameClassCommand(Guid ClassId, string Name) : ICommand;

internal sealed class RenameClassCommandValidator : AbstractValidator<RenameClassCommand>
{
	public RenameClassCommandValidator()
	{
		RuleFor(x => x.ClassId).NotEmpty();
		RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
	}
}

internal sealed class RenameClassCommandHandler(IClassRepository classRepository)
	: ICommandHandler<RenameClassCommand>
{
	public async Task<Result> Handle(
		RenameClassCommand command,
		CancellationToken cancellationToken) =>
		await classRepository
		   .GetAsync(command.ClassId, cancellationToken)
		   .ThenAsync(c => c.Rename(command.Name));
}
