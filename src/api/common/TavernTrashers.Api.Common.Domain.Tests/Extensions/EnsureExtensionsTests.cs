using Shouldly;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;

namespace TavernTrashers.Api.Common.Domain.Tests.Extensions;

public class EnsureExtensionsTests : TestBase
{
	[Fact]
	public void Ensure_With_True_Condition_Should_Return_Original_Result()
	{
		// Arrange
		var result = Result.Success();
		var error = Error.Failure("Test.Error", "Test error");

		// Act
		var ensuredResult = result.Ensure(() => true, error);

		// Assert
		ensuredResult.IsSuccess.ShouldBeTrue();
	}

	[Fact]
	public void Ensure_With_False_Condition_Should_Return_Error()
	{
		// Arrange
		var result = Result.Success();
		var error = Error.Failure("Test.Error", "Test error");

		// Act
		var ensuredResult = result.Ensure(() => false, error);

		// Assert
		ensuredResult.IsFailure.ShouldBeTrue();
		ensuredResult.Error.ShouldBe(error);
	}

	[Fact]
	public void Ensure_With_Value_And_True_Condition_Should_Return_Original_Result()
	{
		// Arrange
		var value = Faker.Random.Int();
		var result = Result.Success(value);
		var error = Error.Failure("Test.Error", "Test error");

		// Act
		var ensuredResult = result.Ensure(() => true, error);

		// Assert
		ensuredResult.IsSuccess.ShouldBeTrue();
		ensuredResult.Value.ShouldBe(value);
	}

	[Fact]
	public void Ensure_With_Value_And_False_Condition_Should_Return_Error()
	{
		// Arrange
		var value = Faker.Random.Int();
		var result = Result.Success(value);
		var error = Error.Failure("Test.Error", "Test error");

		// Act
		var ensuredResult = result.Ensure(() => false, error);

		// Assert
		ensuredResult.IsFailure.ShouldBeTrue();
		ensuredResult.Error.ShouldBe(error);
	}

	[Fact]
	public void Ensure_With_Value_Predicate_And_True_Condition_Should_Return_Original_Result()
	{
		// Arrange
		var value = Faker.Random.Int(1, 100);
		var result = Result.Success(value);
		var error = Error.Failure("Test.Error", "Test error");

		// Act
		var ensuredResult = result.Ensure(v => v > 0, error);

		// Assert
		ensuredResult.IsSuccess.ShouldBeTrue();
		ensuredResult.Value.ShouldBe(value);
	}

	[Fact]
	public void Ensure_With_Value_Predicate_And_False_Condition_Should_Return_Error()
	{
		// Arrange
		var value = Faker.Random.Int(1, 100);
		var result = Result.Success(value);
		var error = Error.Failure("Test.Error", "Value too small");

		// Act
		var ensuredResult = result.Ensure(v => v > 1000, error);

		// Assert
		ensuredResult.IsFailure.ShouldBeTrue();
		ensuredResult.Error.ShouldBe(error);
	}

	[Fact]
	public async Task EnsureAsync_With_Task_Value_And_True_Predicate_Should_Return_Success()
	{
		// Arrange
		var value = Faker.Random.Int(1, 100);
		var taskValue = Task.FromResult(value);
		var error = Error.Failure("Test.Error", "Test error");

		// Act
		var result = await taskValue.EnsureAsync(v => v > 0, error);

		// Assert
		result.IsSuccess.ShouldBeTrue();
		result.Value.ShouldBe(value);
	}

	[Fact]
	public async Task EnsureAsync_With_Task_Value_And_False_Predicate_Should_Return_Error()
	{
		// Arrange
		var value = Faker.Random.Int(1, 100);
		var taskValue = Task.FromResult(value);
		var error = Error.Failure("Test.Error", "Value too small");

		// Act
		var result = await taskValue.EnsureAsync(v => v > 1000, error);

		// Assert
		result.IsFailure.ShouldBeTrue();
		result.Error.ShouldBe(error);
	}
}
