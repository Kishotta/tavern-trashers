using FluentValidation;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Characters.Domain.Classes;

namespace TavernTrashers.Api.Modules.Characters.Application.Classes;

public sealed record DeleteClassCommand(Guid ClassId) : ICommand;

internal sealed class DeleteClassCommandValidator : AbstractValidator<DeleteClassCommand>
{
	public DeleteClassCommandValidator()
	{
		RuleFor(x => x.ClassId).NotEmpty();
	}
}

internal sealed class DeleteClassCommandHandler(IClassRepository classRepository)
	: ICommandHandler<DeleteClassCommand>
{
	public async Task<Result> Handle(
		DeleteClassCommand command,
		CancellationToken cancellationToken) =>
		await classRepository
		   .GetAsync(command.ClassId, cancellationToken)
		   .DoAsync(c => c.Delete());
}
