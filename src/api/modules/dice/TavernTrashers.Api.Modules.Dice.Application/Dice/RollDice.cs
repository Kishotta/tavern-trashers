using TavernTrashers.Api.Common.Application.Clock;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Dice.Application.Abstractions.Data;
using TavernTrashers.Api.Modules.Dice.Domain.DiceEngine;
using TavernTrashers.Api.Modules.Dice.Domain.RecursiveDescentParser;
using TavernTrashers.Api.Modules.Dice.Domain.Rolls;

namespace TavernTrashers.Api.Modules.Dice.Application.Dice;

public sealed record RollDiceCommand(string Expression) : ICommand<RollResponse>;

internal sealed class RollDiceCommandHandler(
	IDiceEngine diceEngine,
	IDateTimeProvider dateTimeProvider,
	IRollRepository rollRepository,
	IUnitOfWork unitOfWork)
	: ICommandHandler<RollDiceCommand, RollResponse>
{
	public async Task<Result<RollResponse>> Handle(RollDiceCommand command, CancellationToken cancellationToken) =>
		await new DiceParser(command.Expression)
		   .ParseExpression()
		   .Then(parser => parser.Evaluate(diceEngine))
		   .Then(outcome => Roll.Create(
				command.Expression,
				outcome,
				dateTimeProvider.UtcNow,
				"{}"))
		   .DoAsync(async roll =>
			{
				rollRepository.Add(roll);

				await unitOfWork.SaveChangesAsync(cancellationToken);
			})
		   .TransformAsync(roll => (RollResponse)roll);
}