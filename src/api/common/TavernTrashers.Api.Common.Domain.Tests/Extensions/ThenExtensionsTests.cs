using Shouldly;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;

namespace TavernTrashers.Api.Common.Domain.Tests.Extensions;

public class ThenExtensionsTests : TestBase
{
	[Fact]
	public void Then_On_Success_Result_Should_Execute_Binder()
	{
		// Arrange
		var result = Result.Success();
		var expectedValue = Faker.Random.Int();

		// Act
		var newResult = result.Then(() => expectedValue);

		// Assert
		newResult.IsSuccess.ShouldBeTrue();
		newResult.Value.ShouldBe(expectedValue);
	}

	[Fact]
	public void Then_On_Failure_Result_Should_Return_Error()
	{
		// Arrange
		var error = Error.Failure("Test.Error", "Test error");
		var result = Result.Failure(error);

		// Act
		var newResult = result.Then(() => Faker.Random.Int());

		// Assert
		newResult.IsFailure.ShouldBeTrue();
		newResult.Error.ShouldBe(error);
	}

	[Fact]
	public void Then_With_Value_Transformation_On_Success_Should_Transform_Value()
	{
		// Arrange
		var originalValue = Faker.Random.Int(1, 100);
		var result = Result.Success(originalValue);

		// Act
		var newResult = result.Then<int, int>(x => x * 2);

		// Assert
		newResult.IsSuccess.ShouldBeTrue();
		newResult.Value.ShouldBe(originalValue * 2);
	}

	[Fact]
	public void Then_With_Value_Transformation_On_Failure_Should_Return_Error()
	{
		// Arrange
		var error = Error.Failure("Test.Error", "Test error");
		var result = Result.Failure<int>(error);

		// Act
		var newResult = result.Then<int, int>(x => x * 2);

		// Assert
		newResult.IsFailure.ShouldBeTrue();
		newResult.Error.ShouldBe(error);
	}

	[Fact]
	public void Then_With_Type_Change_On_Success_Should_Change_Type()
	{
		// Arrange
		var originalValue = Faker.Random.Int();
		var result = Result.Success(originalValue);

		// Act
		var newResult = result.Then(x => x.ToString());

		// Assert
		newResult.IsSuccess.ShouldBeTrue();
		newResult.Value.ShouldBe(originalValue.ToString());
	}

	[Fact]
	public void Then_With_Result_Binder_On_Success_Should_Return_Binder_Result()
	{
		// Arrange
		var originalValue = Faker.Random.Int();
		var result = Result.Success(originalValue);
		var expectedString = originalValue.ToString();

		// Act
		var newResult = result.Then(x => Result.Success(x.ToString()));

		// Assert
		newResult.IsSuccess.ShouldBeTrue();
		newResult.Value.ShouldBe(expectedString);
	}

	[Fact]
	public void Then_With_Result_Binder_On_Success_Can_Return_Failure()
	{
		// Arrange
		var originalValue = Faker.Random.Int();
		var result = Result.Success(originalValue);
		var error = Error.Failure("Test.Error", "Test error");

		// Act
		var newResult = result.Then<int, string>(_ => error);

		// Assert
		newResult.IsFailure.ShouldBeTrue();
		newResult.Error.ShouldBe(error);
	}

	[Fact]
	public async Task ThenAsync_On_Success_Should_Execute_Binder()
	{
		// Arrange
		var originalValue = Faker.Random.Int();
		var resultTask = Task.FromResult(Result.Success(originalValue));
		var expectedString = originalValue.ToString();

		// Act
		var newResult = await resultTask.ThenAsync(x => x.ToString());

		// Assert
		newResult.IsSuccess.ShouldBeTrue();
		newResult.Value.ShouldBe(expectedString);
	}

	[Fact]
	public async Task ThenAsync_On_Failure_Should_Return_Error()
	{
		// Arrange
		var error = Error.Failure("Test.Error", "Test error");
		var resultTask = Task.FromResult(Result.Failure<int>(error));

		// Act
		var newResult = await resultTask.ThenAsync(x => x.ToString());

		// Assert
		newResult.IsFailure.ShouldBeTrue();
		newResult.Error.ShouldBe(error);
	}

	[Fact]
	public async Task ThenAsync_With_Result_Binder_On_Success_Should_Return_Binder_Result()
	{
		// Arrange
		var originalValue = Faker.Random.Int();
		var resultTask = Task.FromResult(Result.Success(originalValue));

		// Act
		var newResult = await resultTask.ThenAsync(x => Result.Success(x.ToString()));

		// Assert
		newResult.IsSuccess.ShouldBeTrue();
		newResult.Value.ShouldBe(originalValue.ToString());
	}

	[Fact]
	public async Task ThenAsync_With_Async_Result_Binder_On_Success_Should_Return_Binder_Result()
	{
		// Arrange
		var originalValue = Faker.Random.Int();
		var resultTask = Task.FromResult(Result.Success(originalValue));

		// Act
		var newResult = await resultTask.ThenAsync(x => Task.FromResult(Result.Success(x.ToString())));

		// Assert
		newResult.IsSuccess.ShouldBeTrue();
		newResult.Value.ShouldBe(originalValue.ToString());
	}

	[Fact]
	public async Task ThenAsync_To_Result_On_Success_Should_Execute_Binder()
	{
		// Arrange
		var originalValue = Faker.Random.Int();
		var resultTask = Task.FromResult(Result.Success(originalValue));

		// Act
		var newResult = await resultTask.ThenAsync(x => Result.Success());

		// Assert
		newResult.IsSuccess.ShouldBeTrue();
	}
}
