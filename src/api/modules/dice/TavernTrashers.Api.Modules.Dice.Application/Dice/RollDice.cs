using FluentValidation;
using TavernTrashers.Api.Common.Application.Clock;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Dice.Domain.DiceEngine;
using TavernTrashers.Api.Modules.Dice.Domain.Rolls;

namespace TavernTrashers.Api.Modules.Dice.Application.Dice;

public sealed record RollDiceCommand(string Expression) : ICommand<RollResponse>;

internal sealed class RollDiceCommandValidator : AbstractValidator<RollDiceCommand>
{
	public RollDiceCommandValidator()
	{
		RuleFor(x => x.Expression).NotEmpty();
	}
}

internal sealed class RollDiceCommandHandler(
	IDiceEngine diceEngine,
	IDateTimeProvider dateTimeProvider,
	IRollRepository rollRepository)
	: ICommandHandler<RollDiceCommand, RollResponse>
{
	public async Task<Result<RollResponse>> Handle(RollDiceCommand command, CancellationToken cancellationToken) =>
		await Task.FromResult(
			command.Expression
			   .ParseDiceExpression()
			   .Then(expression => expression.Evaluate(diceEngine))
			   .Then(outcome => CreateRollEntity(command.Expression, outcome))
			   .Do(rollRepository.Add)
			   .Transform(roll => (RollResponse)roll)
		);

	private Result<Roll> CreateRollEntity(string expression, RollOutcome outcome) =>
		Roll.Create(
			expression,
			outcome,
			dateTimeProvider.UtcNow,
			"{}");
}