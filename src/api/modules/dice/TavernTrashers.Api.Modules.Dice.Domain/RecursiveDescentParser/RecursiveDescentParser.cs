using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Dice.Domain.AbstractSyntaxTree;

namespace TavernTrashers.Api.Modules.Dice.Domain.RecursiveDescentParser;

public class DiceParser
{
	private readonly ReadOnlyMemory<char> _input;
	private int _position;

	public DiceParser(string input)
		: this(input.AsMemory())
	{
	}

	public DiceParser(ReadOnlyMemory<char> input)
	{
		_input = input;
		_position = 0;
	}

	public Result<IExpressionNode> ParseExpression() =>
		ParseTerm()
		   .Then(ParseExpressionRest);

	private Result<IExpressionNode> ParseExpressionRest(IExpressionNode left)
	{
		if (!Match('+') && !Match('-')) return left.ToResult();

		var @operator = _input.Span[_position - 1];
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
			var @operator = _input.Span[_position - 1];
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

		var span = _input.Span[start.._position];
		return int.TryParse(span, out var number)
			? new NumberNode(number)
			: Error.Validation("DiceExpression.InvalidFormat",
				$"Invalid number '{span.ToString()}' at position {start}");
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
		if (PeekChar() == 'f')
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
		var span = _input.Span;
		return _position < span.Length ? char.ToLowerInvariant(span[_position]) : '\0';
	}

	private bool PeekIsDigit()
	{
		SkipWhitespace();
		return char.IsDigit(PeekChar());
	}

	private int Advance() =>
		_position < _input.Length
			? _position++
			: -1;

	private void SkipWhitespace()
	{
		var span = _input.Span;
		while (_position < span.Length && char.IsWhiteSpace(span[_position]))
			_position++;
	}

	private bool Match(char c)
	{
		SkipWhitespace();
		return char.ToLowerInvariant(PeekChar()) == char.ToLowerInvariant(c) && Advance() >= 0;
	}

	private Result Expect(char c) =>
		Match(c)
			? Unit.Value
			: Error.Validation(
				"DiceExpression.InvalidFormat",
				$"Expected '{c}' at position {_position}");
}