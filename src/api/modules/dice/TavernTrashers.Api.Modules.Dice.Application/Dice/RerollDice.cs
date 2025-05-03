using TavernTrashers.Api.Common.Application.Clock;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Dice.Application.Abstractions.Data;
using TavernTrashers.Api.Modules.Dice.Domain.AbstractSyntaxTree;
using TavernTrashers.Api.Modules.Dice.Domain.DiceEngine;
using TavernTrashers.Api.Modules.Dice.Domain.RecursiveDescentParser;
using TavernTrashers.Api.Modules.Dice.Domain.Rolls;

namespace TavernTrashers.Api.Modules.Dice.Application.Dice;

public sealed record RerollDiceCommand(
	Guid RollId,
	IReadOnlyList<int> DiceIndices) : ICommand<RollResponse>;

internal sealed class RerollDiceCommandHandler(
	IRollRepository rollRepository,
	IDiceEngine fallbackDiceEngine,
	IDateTimeProvider dateTimeProvider,
	IUnitOfWork unitOfWork)
	: ICommandHandler<RerollDiceCommand, RollResponse>
{
	public async Task<Result<RollResponse>> Handle(RerollDiceCommand command, CancellationToken cancellationToken) =>
		await rollRepository.GetAsync(command.RollId, cancellationToken)
		   .ThenAsync(ParseExpression)
		   .ThenAsync(ReevaluateExpression(command))
		   .ThenAsync(CreateReroll())
		   .DoAsync(async roll =>
			{
				rollRepository.Add(roll);

				await unitOfWork.SaveChangesAsync(cancellationToken);
			})
		   .TransformAsync(roll => (RollResponse)roll);

	private static Result<(Roll Original, IExpressionNode Ast)> ParseExpression(Roll original) =>
		new DiceParser(original.Expression).ParseExpression()
		   .Transform(ast => (Original: original, Ast: ast));

	private Func<(Roll Original, IExpressionNode Ast), Result<(RollOutcome Outcome, Roll Original)>>
		ReevaluateExpression(RerollDiceCommand command) =>
		tuple =>
		{
			var rerollDiceEngine = new RerollDiceEngine(
				tuple.Original.RawRolls.ToList(),
				command.DiceIndices,
				fallbackDiceEngine);
			return tuple.Ast.Evaluate(rerollDiceEngine)
			   .Transform(outcome => (Outcome: outcome, tuple.Original));
		};

	private Func<(RollOutcome Outcome, Roll Original), Roll> CreateReroll() =>
		tuple => Roll.Reroll(
			tuple.Original,
			tuple.Outcome,
			dateTimeProvider.UtcNow
		);
}