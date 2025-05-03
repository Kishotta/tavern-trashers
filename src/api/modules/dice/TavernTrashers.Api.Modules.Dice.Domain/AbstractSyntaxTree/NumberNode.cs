using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Modules.Dice.Domain.AbstractSyntaxTree;

/// <summary>
///     A literal number: "42"
/// </summary>
public class NumberNode(int value)
	: IExpressionNode
{
	public int Value { get; } = value;
	public Result<Roll> Evaluate(IDiceEngine diceEngine) => Roll.Constant(Value);
}