using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Dice.Domain.Rolls;

namespace TavernTrashers.Api.Modules.Dice.Domain.AbstractSyntaxTree;

/// <summary>
///     A binary operation: left (+|-|*|/) right
/// </summary>
public record BinaryOperationNode(
	IExpressionNode leftNode,
	char @operator,
	IExpressionNode rightNode)
	: IExpressionNode
{
	public Result<RollOutcome> Evaluate(IDiceEngine diceEngine) =>
		leftNode
		   .Evaluate(diceEngine)
		   .Then(left => rightNode
			   .Evaluate(diceEngine)
			   .Transform<RollOutcome, Result<RollOutcome>>(right =>
				{
					try
					{
						var total = (Result<int>)(@operator switch
						{
							'+' => left.Total + right.Total,
							'-' => left.Total - right.Total,
							'*' => left.Total * right.Total,
							'/' => left.Total / right.Total,
							_ => Error.Validation(
								"DiceExpression.InvalidOperator",
								$"Invalid operator '{@operator}' used in expression."),
						});
						if (total.IsFailure) return total.Error;

						var average = (Result<double>)(@operator switch
						{
							'+' => left.Average + right.Average,
							'-' => left.Average - right.Average,
							'*' => left.Average * right.Average,
							'/' => left.Average / right.Average,
							_ => Error.Validation(
								"DiceExpression.InvalidOperator",
								$"Invalid operator '{@operator}' used in expression."),
						});
						if (average.IsFailure) return average.Error;

						int minimum, maximum;
						switch (@operator)
						{
							case '+':
								minimum = left.Minimum + right.Minimum;
								maximum = left.Maximum + right.Maximum;

								break;

							case '-':
								minimum = left.Minimum - right.Maximum;
								maximum = left.Maximum - right.Minimum;

								break;

							case '*':
								var products = new[]
								{
									left.Minimum * right.Minimum,
									left.Minimum * right.Maximum,
									left.Maximum * right.Minimum,
									left.Maximum * right.Maximum,
								};
								minimum = products.Min();
								maximum = products.Max();
								break;

							case '/':
								var quotients = new[]
								{
									left.Minimum / right.Minimum,
									left.Minimum / right.Maximum,
									left.Maximum / right.Minimum,
									left.Maximum / right.Maximum,
								};

								minimum = quotients.Min();
								maximum = quotients.Max();

								break;

							default:
								return Error.Validation(
									"DiceExpression.InvalidOperator",
									$"Invalid operator '{@operator}' used in expression.");
						}

						return new RollOutcome(
							total,
							minimum,
							maximum,
							average,
							left.RawRolls.Concat(right.RawRolls).ToList(),
							left.KeptRolls.Concat(right.KeptRolls).ToList());
					}
					catch (DivideByZeroException)
					{
						return Error.Problem(
							"DiceExpression.DivisionByZero",
							"Division by zero encountered in expression.");
					}
				}));
}