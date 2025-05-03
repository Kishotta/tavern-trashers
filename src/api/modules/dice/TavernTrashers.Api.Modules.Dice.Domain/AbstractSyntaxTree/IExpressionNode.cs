using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Modules.Dice.Domain.Rolls;

namespace TavernTrashers.Api.Modules.Dice.Domain.AbstractSyntaxTree;

/// <summary>
///     Base interface for all expression nodes in the abstract syntax tree.
/// </summary>
public interface IExpressionNode
{
	Result<RollOutcome> Evaluate(IDiceEngine diceEngine);
}