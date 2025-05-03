using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Modules.Dice.Domain;
using TavernTrashers.Api.Modules.Dice.Domain.RecursiveDescentParser;

namespace TavernTrashers.Api.Modules.Dice.Application.Dice;

public sealed record RollDiceCommand(string Expression) : ICommand<DiceRollResponse>;

internal sealed class RollDiceCommandHandler(IDiceEngine diceEngine)
	: ICommandHandler<RollDiceCommand, DiceRollResponse>
{
	public async Task<Result<DiceRollResponse>> Handle(RollDiceCommand command, CancellationToken cancellationToken)
	{
		var parser = new DiceParser(command.Expression).ParseExpression();
		if (parser.IsFailure)
			return parser.Error;

		var evaluation = await Task.Run(() =>
			parser.Value.Evaluate(diceEngine), cancellationToken);

		if (evaluation.IsFailure)
			return evaluation.Error;

		return new DiceRollResponse(
			Guid.NewGuid(),
			command.Expression,
			evaluation.Value.Total,
			evaluation.Value.Minimum,
			evaluation.Value.Maximum,
			evaluation.Value.Average,
			evaluation.Value.RawRolls,
			evaluation.Value.KeptRolls,
			DateTime.UtcNow);
	}
}

public sealed record DiceRollResponse(
	Guid Id,
	string Expression,
	int Total,
	int Minimum,
	int Maximum,
	double Average,
	IReadOnlyList<int> RawRolls,
	IReadOnlyList<int> KeptRolls,
	DateTime RolledAtUtc);