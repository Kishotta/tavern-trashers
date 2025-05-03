using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Modules.Dice.Domain.AbstractSyntaxTree;

/// <summary>
///     Base interface for all expression nodes in the abstract syntax tree.
/// </summary>
public interface IExpressionNode
{
	Result<Roll> Evaluate(IDiceEngine diceEngine);
}