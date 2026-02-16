using Shouldly;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;

namespace TavernTrashers.Api.Common.Domain.Tests.Extensions;

public class MatchExtensionsTests : TestBase
{
	[Fact]
	public void Match_On_Success_Result_Should_Execute_OnSuccess()
	{
		// Arrange
		var result = Result.Success();
		var successValue = Faker.Random.Int();
		var failureValue = Faker.Random.Int();

		// Act
		var output = result.Match(
			() => successValue,
			_ => failureValue);

		// Assert
		output.ShouldBe(successValue);
	}

	[Fact]
	public void Match_On_Failure_Result_Should_Execute_OnFailure()
	{
		// Arrange
		var error = Error.Failure("Test.Error", "Test error");
		var result = Result.Failure(error);
		var successValue = Faker.Random.Int();
		var failureValue = Faker.Random.Int();

		// Act
		var output = result.Match(
			() => successValue,
			_ => failureValue);

		// Assert
		output.ShouldBe(failureValue);
	}

	[Fact]
	public void Match_With_Value_On_Success_Should_Execute_OnSuccess()
	{
		// Arrange
		var value = Faker.Random.Int();
		var result = Result.Success(value);

		// Act
		var output = result.Match(
			x => x.ToString(),
			_ => "failure");

		// Assert
		output.ShouldBe(value.ToString());
	}

	[Fact]
	public void Match_With_Value_On_Failure_Should_Execute_OnFailure()
	{
		// Arrange
		var error = Error.Failure("Test.Error", "Test error");
		var result = Result.Failure<int>(error);

		// Act
		var output = result.Match(
			x => x.ToString(),
			_ => "failure");

		// Assert
		output.ShouldBe("failure");
	}

	[Fact]
	public async Task MatchAsync_On_Success_Should_Execute_OnSuccess()
	{
		// Arrange
		var resultTask = Task.FromResult(Result.Success());
		var successValue = Faker.Random.Int();
		var failureValue = Faker.Random.Int();

		// Act
		var output = await resultTask.MatchAsync(
			() => successValue,
			_ => failureValue);

		// Assert
		output.ShouldBe(successValue);
	}

	[Fact]
	public async Task MatchAsync_On_Failure_Should_Execute_OnFailure()
	{
		// Arrange
		var error = Error.Failure("Test.Error", "Test error");
		var resultTask = Task.FromResult(Result.Failure(error));
		var successValue = Faker.Random.Int();
		var failureValue = Faker.Random.Int();

		// Act
		var output = await resultTask.MatchAsync(
			() => successValue,
			_ => failureValue);

		// Assert
		output.ShouldBe(failureValue);
	}

	[Fact]
	public async Task MatchAsync_With_Value_On_Success_Should_Execute_OnSuccess()
	{
		// Arrange
		var value = Faker.Random.Int();
		var resultTask = Task.FromResult(Result.Success(value));

		// Act
		var output = await resultTask.MatchAsync(
			x => x.ToString(),
			_ => "failure");

		// Assert
		output.ShouldBe(value.ToString());
	}

	[Fact]
	public async Task MatchAsync_With_Value_On_Failure_Should_Execute_OnFailure()
	{
		// Arrange
		var error = Error.Failure("Test.Error", "Test error");
		var resultTask = Task.FromResult(Result.Failure<int>(error));

		// Act
		var output = await resultTask.MatchAsync(
			x => x.ToString(),
			_ => "failure");

		// Assert
		output.ShouldBe("failure");
	}
}
