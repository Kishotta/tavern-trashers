using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Dice.Domain.AbstractSyntaxTree;

namespace TavernTrashers.Api.Modules.Dice.Domain.RecursiveDescentParser;

public class DiceParser(string input)
{
	private int _position;

	public Result<IExpressionNode> ParseExpression() =>
		ParseTerm()
		   .Then(ParseExpressionRest);

	private Result<IExpressionNode> ParseExpressionRest(IExpressionNode left)
	{
		if (!Match('+') && !Match('-')) return left.ToResult();

		var @operator = input[_position - 1];
		return ParseTerm()
		   .Then(right => ParseExpressionRest(
				new BinaryOperationNode(left, @operator, right)));
	}

	private Result<IExpressionNode> ParseTerm()
		=> ParseFactor()
		   .Then(ParseTermRest);

	private Result<IExpressionNode> ParseTermRest(IExpressionNode left)
	{
		if (Match('*') || Match('/'))
		{
			var @operator = input[_position - 1];
			return ParseFactor()
			   .Then(right => ParseTermRest(
					new BinaryOperationNode(left, @operator, right)));
		}

		return left.ToResult();
	}

	private Result<IExpressionNode> ParseFactor()
	{
		SkipWhitespace();
		if (Match('('))
			return ParseExpression()
			   .Then(expression => Expect(')')
				   .Then(() => expression));

		if (PeekIsDigit() || PeekChar() == 'd')
		{
			var save       = _position;
			var diceResult = ParseDiceRoll();
			if (diceResult.IsSuccess)
				return diceResult.Transform(d => (IExpressionNode)d);

			_position = save;
		}

		if (PeekIsDigit())
			return ParseNumber().Transform(IExpressionNode (number) => number);

		return Error.Validation("DiceExpression.InvalidFormat",
			$"Expected a number or 'd' at position {_position}");
	}

	private Result<NumberNode> ParseNumber()
	{
		var start = _position;
		while (PeekIsDigit()) Advance();

		if (start == _position)
			return Error.Validation(
				"DiceExpression.InvalidFormat",
				$"Expected a number at position {_position}");

		var text = input[start.._position];
		return int.TryParse(text, out var number)
			? new NumberNode(number)
			: Error.Validation("DiceExpression.InvalidFormat",
				$"Invalid number '{text}' at position {start}");
	}

	private Result<DiceRollNode> ParseDiceRoll()
	{
		// [count]
		var count = 1;
		if (PeekIsDigit())
		{
			var countResult = ParseNumber();
			if (countResult.IsFailure) return countResult.Error;
			count = countResult.Value.Value;
		}

		if (!Match('d'))
			return Error.Validation(
				"DiceExpression.InvalidFormat",
				$"Expected 'd' at position {_position}");

		// sides
		int sides;
		if (PeekChar() == 'f' || PeekChar() == 'F')
		{
			sides = 0;
			Advance();
		}
		else
		{
			var sidesResult = ParseNumber();
			if (sidesResult.IsFailure) return sidesResult.Error;
			sides = sidesResult.Value.Value;
		}

		// explode?
		var explode = Match('!');

		// keep/drop?
		var mode      = KeepDropMode.None;
		var modeCount = 0;
		if (Match('k'))
		{
			var high = Match('h');
			if (!high && !Match('l'))
				return Error.Validation("DiceExpression.InvalidFormat",
					$"Expected 'h' or 'l' after 'k' at position {_position}");
			mode = high ? KeepDropMode.KeepHighest : KeepDropMode.KeepLowest;

			modeCount = 1;
			if (PeekIsDigit())
			{
				var modeCountResult = ParseNumber();
				if (modeCountResult.IsFailure) return modeCountResult.Error;
				modeCount = modeCountResult.Value.Value;
			}
		}
		else if (Match('d'))
		{
			var high = Match('h');
			if (!high && !Match('l'))
				return Error.Validation("DiceExpression.InvalidFormat",
					$"Expected 'h' or 'l' after 'd' at position {_position}");
			mode = high ? KeepDropMode.DropHighest : KeepDropMode.DropLowest;

			modeCount = 1;
			if (PeekIsDigit())
			{
				var modeCountResult = ParseNumber();
				if (modeCountResult.IsFailure) return modeCountResult.Error;
				modeCount = modeCountResult.Value.Value;
			}
		}

		return new DiceRollNode(
			count,
			sides,
			explode,
			mode,
			modeCount);
	}

	private char PeekChar()
	{
		SkipWhitespace();
		return _position < input.Length ? input[_position] : '\0';
	}

	private bool PeekIsDigit()
	{
		SkipWhitespace();
		return char.IsDigit(PeekChar());
	}

	private int Advance() =>
		_position < input.Length
			? _position++
			: -1;

	private void SkipWhitespace()
	{
		while (_position < input.Length && char.IsWhiteSpace(input[_position]))
			_position++;
	}

	private bool Match(char c)
	{
		SkipWhitespace();
		return PeekChar() == c && Advance() >= 0;
	}

	private Result Expect(char c) =>
		Match(c)
			? Unit.Value
			: Error.Validation(
				"DiceExpression.InvalidFormat",
				$"Expected '{c}' at position {_position}");
}