using Shouldly;
using TavernTrashers.Api.Modules.Dice.Domain.DiceEngine;
using TavernTrashers.Api.Modules.Dice.Domain.RecursiveDescentParser;
using TavernTrashers.Api.Modules.Dice.Domain.Rolls;

namespace TavernTrashers.Api.Modules.Dice.Domain.Tests;

public class DiceParserEvaluatorTests
{
	public static IEnumerable<object[]> ParsesAndComputesBoundsCorrectlyData => new List<object[]>
	{
		new object[] { "d20", 1, 20, new[] { new DieResult(1, "20") } },
		new object[] { "2d6", 2, 12, new[] { new DieResult(1, "6"), new DieResult(1, "6") } },
		new object[] { "2d20kh", 1, 20, new[] { new DieResult(1, "20"), new DieResult(1, "20") } },
		new object[] { "4d6kh3", 3, 18, new[] { new DieResult(1, "6"), new DieResult(1, "6"), new DieResult(1, "6"), new DieResult(1, "6") } },
		new object[] { "d20+d4", 2, 24, new[] { new DieResult(1, "20"), new DieResult(1, "4") } },
	};

	[Theory]
	[MemberData(nameof(ParsesAndComputesBoundsCorrectlyData))]
	public void ParsesAndComputesBoundsCorrectly(
		string expression,
		int expectedMinimum,
		int expectedMaximum,
		IEnumerable<DieResult> rolls)
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
		var stub = new StubEngine([new(7, "20"), new(11, "20")]);

		var outcome = astRes.Value.Evaluate(stub).Value;

		outcome.KeptRolls.ShouldBeEquivalentTo(new List<DieResult> { new(11, "20") });
		outcome.Minimum.ShouldBe(1);
		outcome.Maximum.ShouldBe(20);
	}

	[Fact]
	public void Subtraction_Bounds_ReversedCorrectly()
	{
		var parser = new DiceParser("1-d4");
		var astRes = parser.ParseExpression();
		astRes.IsSuccess.ShouldBeTrue();
		var stub = new StubEngine([new(4, "4")]);

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
		var stub = new StubEngine([new(6, "6"), new(6, "6"), new(5, "6")]);

		var outcome = astRes.Value.Evaluate(stub).Value;

		outcome.Minimum.ShouldBe(3);
		outcome.Maximum.ShouldBe(18);
		outcome.KeptRolls.ShouldBeEquivalentTo(new List<DieResult> { new(6, "6"), new(6, "6"), new(5, "6") });
	}

	[Fact]
	public void Explode_Dice_WithKeep_Works()
	{
		var parser = new DiceParser("3d6kh3!");
		var astRes = parser.ParseExpression();
		astRes.IsSuccess.ShouldBeTrue();
		var stub = new StubEngine([new(6, "6"), new(6, "6"), new(3, "6"), new(4, "6"), new(5, "6")]);

		var outcome = astRes.Value.Evaluate(stub).Value;

		outcome.Minimum.ShouldBe(3);
		outcome.Maximum.ShouldBe(18);
		outcome.KeptRolls.ShouldBeEquivalentTo(new List<DieResult> { new(6, "6"), new(6, "6"), new(3, "6") });
	}

	[Fact]
	public void Whitespace_Ignored()
	{
		var parser = new DiceParser("  2 d 20 kh  ");
		var astRes = parser.ParseExpression();
		astRes.IsSuccess.ShouldBeTrue();
		var stub = new StubEngine([new(7, "20"), new(11, "20")]);

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
		var stub = new StubEngine([new(3, "6"), new(5, "6"), new(2, "4")]);

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

	public static IEnumerable<object[]> DivisionByZeroData => new List<object[]>
	{
		new object[] { "1/0", Array.Empty<DieResult>() },
		new object[] { "d6/(2-d4)", new[] { new DieResult(6, "6"), new DieResult(2, "4") } },
	};

	[Theory]
	[MemberData(nameof(DivisionByZeroData))]
	public void Division_By_Zero_Returns_Error(string expression, IEnumerable<DieResult> rolls)
	{
		var parser = new DiceParser(expression);
		var astRes = parser.ParseExpression();
		astRes.IsSuccess.ShouldBeTrue();
		var stub = new StubEngine(rolls);

		var outcome = astRes.Value.Evaluate(stub);

		outcome.IsSuccess.ShouldBeFalse();
		outcome.Error.Code.ShouldBe("DiceExpression.DivisionByZero");
	}

	[Fact]
	public void Exploding_Fate_Dice_Should_Explode_On_One()
	{
		var parser = new DiceParser("1df!");
		var astRes = parser.ParseExpression();
		astRes.IsSuccess.ShouldBeTrue();
		// Provide a fate die result of 1 (max), then 0 (not max)
		var stub = new StubEngine([new(1, "f"), new(0, "f")]);

		var outcome = astRes.Value.Evaluate(stub).Value;

		// Should explode once, so two rolls in total
		outcome.RawRolls.ShouldBeEquivalentTo(new List<DieResult> { new(1, "f"), new(0, "f") });
		outcome.KeptRolls.ShouldBeEquivalentTo(new List<DieResult> { new(1, "f"), new(0, "f") });
	}

	[Fact]
	public void Exploding_Fate_Dice_Should_Not_Explode_On_Zero_Or_Negative_One()
	{
		var parser = new DiceParser("1df!");
		var astRes = parser.ParseExpression();
		astRes.IsSuccess.ShouldBeTrue();
		// Provide a fate die result of 0 (not max)
		var stub = new StubEngine(new[] { new DieResult(0, "f") });

		var outcome = astRes.Value.Evaluate(stub).Value;

		outcome.RawRolls.ShouldBeEquivalentTo(new List<DieResult> { new(0, "f") });
		outcome.KeptRolls.ShouldBeEquivalentTo(new List<DieResult> { new(0, "f") });

		// Provide a fate die result of -1 (not max)
		stub    = new(new[] { new DieResult(-1, "f") });
		outcome = astRes.Value.Evaluate(stub).Value;
		outcome.RawRolls.ShouldBeEquivalentTo(new List<DieResult> { new(-1, "f") });
		outcome.KeptRolls.ShouldBeEquivalentTo(new List<DieResult> { new(-1, "f") });
	}

	private class StubEngine(IEnumerable<DieResult> rolls) : IDiceEngine
	{
		private readonly Queue<DieResult> _rolls = new(rolls);

		public DieResult Roll(int sides) => _rolls.Dequeue();
	}
}