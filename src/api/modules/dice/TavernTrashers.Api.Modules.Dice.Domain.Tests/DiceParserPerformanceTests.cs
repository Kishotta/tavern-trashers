using System.Diagnostics;
using Shouldly;
using TavernTrashers.Api.Modules.Dice.Domain.RecursiveDescentParser;
using Xunit.Abstractions;

namespace TavernTrashers.Api.Modules.Dice.Domain.Tests;

public class DiceParserPerformanceTests(ITestOutputHelper output)
{
	private readonly ITestOutputHelper _output = output;

	[Fact]
	public void Parser_Should_Handle_Large_Number_Of_Parses_Efficiently()
	{
		// Arrange
		const int iterations = 10_000;
		var expressions = new[]
		{
			"2d6+5",
			"4d6kh3",
			"d20+d4",
			"2d20kh1+1d8+3",
			"  3 d 6 kh 2  ",
			"1d20!+5",
			"4d6dh1"
		};

		// Act
		var sw = Stopwatch.StartNew();
		for (var i = 0; i < iterations; i++)
		{
			foreach (var expr in expressions)
			{
				var parser = new DiceParser(expr);
				var result = parser.ParseExpression();
				result.IsSuccess.ShouldBeTrue();
			}
		}
		sw.Stop();

		// Assert - Performance validation
		var totalParses = iterations * expressions.Length;
		var avgMicroseconds = (sw.Elapsed.TotalMicroseconds / totalParses);
		
		_output.WriteLine($"Total parses: {totalParses:N0}");
		_output.WriteLine($"Total time: {sw.ElapsedMilliseconds:N0}ms");
		_output.WriteLine($"Average per parse: {avgMicroseconds:F2}μs");
		_output.WriteLine($"Parses per second: {totalParses / sw.Elapsed.TotalSeconds:N0}");

		// With Span<char> optimizations, parsing should be very fast
		// This is just a smoke test - actual benchmarks would use BenchmarkDotNet
		sw.ElapsedMilliseconds.ShouldBeLessThan(5000); // Should complete in under 5 seconds
	}

	[Fact]
	public void Parser_ReadOnlyMemory_Constructor_Works()
	{
		// Arrange
		var expression = "2d6+5".AsMemory();

		// Act
		var parser = new DiceParser(expression);
		var result = parser.ParseExpression();

		// Assert
		result.IsSuccess.ShouldBeTrue();
	}

	[Fact]
	public void Parser_Can_Parse_From_String_Slice()
	{
		// Arrange - demonstrate zero-allocation parsing from a slice
		var fullString = "IGNORE:2d6+5:IGNORE";
		var slice = fullString.AsMemory(7, 6); // Just "2d6+5"

		// Act
		var parser = new DiceParser(slice);
		var result = parser.ParseExpression();

		// Assert
		result.IsSuccess.ShouldBeTrue();
	}
}
