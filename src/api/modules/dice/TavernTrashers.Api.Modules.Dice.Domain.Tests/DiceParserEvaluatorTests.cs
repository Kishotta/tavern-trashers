using Shouldly;
using TavernTrashers.Api.Modules.Dice.Domain.DiceEngine;
using TavernTrashers.Api.Modules.Dice.Domain.RecursiveDescentParser;

namespace TavernTrashers.Api.Modules.Dice.Domain.Tests;

public class DiceParserEvaluatorTests
{
	[Theory]
	[InlineData("d20", 1, 20, 1)]
	[InlineData("2d6", 2, 12, 1, 1)]
	[InlineData("2d20kh", 1, 20, 1, 1)]
	[InlineData("4d6kh3", 3, 18, 1, 1, 1, 1)]
	[InlineData("d20+d4", 2, 24, 1, 1)]
	public void ParsesAndComputesBoundsCorrectly(
		string expression,
		int expectedMinimum,
		int expectedMaximum,
		params int[] rolls)
	{
		var parser = new DiceParser(expression);
		var astRes = parser.ParseExpression();
		astRes.IsSuccess.ShouldBeTrue();

		var stub    = new StubEngine(rolls);
		var outcome = astRes.Value.Evaluate(stub).Value;

		outcome.Minimum.ShouldBe(expectedMinimum);
		outcome.Maximum.ShouldBe(expectedMaximum);
	}

	[Fact]
	public void KeepDrop_DefaultCount_Works()
	{
		var parser = new DiceParser("2d20kh");
		var astRes = parser.ParseExpression();
		astRes.IsSuccess.ShouldBeTrue();
		var stub = new StubEngine(7, 11);

		var outcome = astRes.Value.Evaluate(stub).Value;

		outcome.KeptRolls.ShouldBeEquivalentTo(new List<int> { 11 });
		outcome.Minimum.ShouldBe(1);
		outcome.Maximum.ShouldBe(20);
	}

	[Fact]
	public void Subtraction_Bounds_ReversedCorrectly()
	{
		var parser = new DiceParser("1-d4");
		var astRes = parser.ParseExpression();
		astRes.IsSuccess.ShouldBeTrue();
		var stub = new StubEngine(4);

		var outcome = astRes.Value.Evaluate(stub).Value;

		outcome.Minimum.ShouldBe(-3);
		outcome.Maximum.ShouldBe(0);
	}

	// and so on for explode, whitespace, combined expressions, invalid syntax, etc.
	[Fact]
	public void Explode_Dice_Works()
	{
		var parser = new DiceParser("1d6!");
		var astRes = parser.ParseExpression();
		astRes.IsSuccess.ShouldBeTrue();
		var stub = new StubEngine(6, 6, 5); // The first two rolls are 6, which should explode

		var outcome = astRes.Value.Evaluate(stub).Value;

		outcome.Minimum.ShouldBe(3);
		outcome.Maximum.ShouldBe(18);
		outcome.KeptRolls.ShouldBeEquivalentTo(new List<int> { 6, 6, 5 });
	}

	[Fact]
	public void Explode_Dice_WithKeep_Works()
	{
		var parser = new DiceParser("3d6kh3!");
		var astRes = parser.ParseExpression();
		astRes.IsSuccess.ShouldBeTrue();
		var stub = new StubEngine(6, 6, 3, 4, 5); // The first two rolls are 6, which

		var outcome = astRes.Value.Evaluate(stub).Value;

		outcome.Minimum.ShouldBe(3);
		outcome.Maximum.ShouldBe(18);
		outcome.KeptRolls.ShouldBeEquivalentTo(new List<int>
			{ 6, 6, 3 }); // Not sure if this is how this expression is expected to work
	}

	[Fact]
	public void Whitespace_Ignored()
	{
		var parser = new DiceParser("  2 d 20 kh  ");
		var astRes = parser.ParseExpression();
		astRes.IsSuccess.ShouldBeTrue();
		var stub = new StubEngine(7, 11);

		var outcome = astRes.Value.Evaluate(stub).Value;

		outcome.Total.ShouldBe(11);
		outcome.Minimum.ShouldBe(1);
		outcome.Maximum.ShouldBe(20);
	}

	[Fact]
	public void Combined_Expressions_Works()
	{
		var parser = new DiceParser("2d6 + 1d4 - 3");
		var astRes = parser.ParseExpression();
		astRes.IsSuccess.ShouldBeTrue();
		var stub = new StubEngine(3, 5, 2); // Rolls for 2d6 are 3 and 5, and for 1d4 is 2

		var outcome = astRes.Value.Evaluate(stub).Value;

		outcome.Total.ShouldBe(7);    // (8 + 2 - 3) = 7
		outcome.Minimum.ShouldBe(0);  // (2 + 1 - 3) = 0
		outcome.Maximum.ShouldBe(13); // (12 + 4 - 3) = 13
	}

	[Theory]
	[InlineData("2d6 + 1d4 -")]
	[InlineData("2d6 + 1d4 - 3 +")]
	public void Invalid_Syntax_Returns_Error(string expression)
	{
		var parser = new DiceParser(expression);
		var astRes = parser.ParseExpression();

		astRes.IsSuccess.ShouldBeFalse();
		astRes.Error.Code.ShouldBe("DiceExpression.InvalidFormat");
	}

	[Theory]
	[InlineData("1/0")]
	[InlineData("d6/(2-d4)", 6, 2)]
	public void Division_By_Zero_Returns_Error(string expression, params int[] rolls)
	{
		var parser = new DiceParser(expression);
		var astRes = parser.ParseExpression();
		astRes.IsSuccess.ShouldBeTrue();
		var stub = new StubEngine(rolls);

		var outcome = astRes.Value.Evaluate(stub);

		outcome.IsSuccess.ShouldBeFalse();
		outcome.Error.Code.ShouldBe("DiceExpression.DivisionByZero");
	}

	private class StubEngine(params IEnumerable<int> rolls) : IDiceEngine
	{
		private readonly Queue<int> _rolls = new(rolls);
		public int Roll(int sides) => _rolls.Dequeue();
	}
}