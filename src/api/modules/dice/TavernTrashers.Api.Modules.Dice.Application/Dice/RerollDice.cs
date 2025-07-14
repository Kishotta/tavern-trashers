using FluentValidation;
using TavernTrashers.Api.Common.Application.Clock;
using TavernTrashers.Api.Common.Application.Data;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Dice.Application.Abstractions.Data;
using TavernTrashers.Api.Modules.Dice.Domain.AbstractSyntaxTree;
using TavernTrashers.Api.Modules.Dice.Domain.DiceEngine;
using TavernTrashers.Api.Modules.Dice.Domain.Rolls;

namespace TavernTrashers.Api.Modules.Dice.Application.Dice;

public sealed record RerollDiceCommand(
	Guid RollId,
	IReadOnlyList<int> DiceIndices) : ICommand<RollResponse>;

internal sealed class RerollDiceCommandValidator : AbstractValidator<RerollDiceCommand>
{
	public RerollDiceCommandValidator()
	{
		RuleFor(command => command.RollId).NotEmpty();
		RuleFor(command => command.DiceIndices)
		   .NotEmpty()
		   .WithMessage("At least one dice index must be specified.")
		   .Must(BeValidArrayIndices)
		   .WithMessage("All dice indices must be non-negative.");
	}

	private static bool BeValidArrayIndices(IReadOnlyList<int> indices) => indices.All(index => index >= 0);
}

internal sealed class RerollDiceCommandHandler(
	IRollRepository rollRepository,
	IDiceEngine fallbackDiceEngine,
	IDateTimeProvider dateTimeProvider,
	IUnitOfWork unitOfWork)
	: ICommandHandler<RerollDiceCommand, RollResponse>
{
	public async Task<Result<RollResponse>> Handle(RerollDiceCommand command, CancellationToken cancellationToken) =>
		await rollRepository
		   .GetAsync(command.RollId, cancellationToken)
		   .ThenAsync(originalRoll =>
				originalRoll.DiceExpression
				   .Then(originalDiceExpression => ReevaluateOriginalExpression(originalRoll, originalDiceExpression, command))
				   .Then(newOutcome => CreateRerollEntity(newOutcome, originalRoll)))
		   .DoAsync(rollRepository.Add)
		   .SaveChangesAsync(unitOfWork, cancellationToken)
		   .TransformAsync(roll => (RollResponse)roll);

	private Result<RollOutcome> ReevaluateOriginalExpression(
		Roll originalRoll,
		IExpressionNode originalDiceExpression,
		RerollDiceCommand command) =>
		originalDiceExpression
		   .Evaluate(new RerollDiceEngine(
				originalRoll.RawRolls.ToList(),
				command.DiceIndices,
				fallbackDiceEngine));

	private Result<Roll> CreateRerollEntity(RollOutcome newOutcome, Roll originalRoll) =>
		Roll.Reroll(
			originalRoll,
			newOutcome,
			dateTimeProvider.UtcNow);
}