using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Modules.Dice.Domain.Rolls;

namespace TavernTrashers.Api.Modules.Dice.Domain.AbstractSyntaxTree;

/// <summary>
///     A literal number: "42"
/// </summary>
public class NumberNode(int value)
	: IExpressionNode
{
	public int Value { get; } = value;
	public Result<RollOutcome> Evaluate(IDiceEngine diceEngine) => RollOutcome.Constant(Value);
}