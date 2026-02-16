using Shouldly;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;

namespace TavernTrashers.Api.Common.Domain.Tests.Extensions;

public class DoExtensionsTests : TestBase
{
	[Fact]
	public void Do_On_Success_Should_Execute_Action()
	{
		// Arrange
		var result = Result.Success();
		var actionExecuted = false;

		// Act
		var returnedResult = result.Do(() => actionExecuted = true);

		// Assert
		actionExecuted.ShouldBeTrue();
		returnedResult.ShouldBe(result);
	}

	[Fact]
	public void Do_On_Failure_Should_Not_Execute_Action()
	{
		// Arrange
		var error = Error.Failure("Test.Error", "Test error");
		var result = Result.Failure(error);
		var actionExecuted = false;

		// Act
		var returnedResult = result.Do(() => actionExecuted = true);

		// Assert
		actionExecuted.ShouldBeFalse();
		returnedResult.ShouldBe(result);
	}

	[Fact]
	public void Do_With_Value_On_Success_Should_Execute_Action_With_Value()
	{
		// Arrange
		var value = Faker.Random.Int();
		var result = Result.Success(value);
		var capturedValue = 0;

		// Act
		var returnedResult = result.Do(v => capturedValue = v);

		// Assert
		capturedValue.ShouldBe(value);
		returnedResult.ShouldBe(result);
	}

	[Fact]
	public void Do_With_Value_On_Failure_Should_Not_Execute_Action()
	{
		// Arrange
		var error = Error.Failure("Test.Error", "Test error");
		var result = Result.Failure<int>(error);
		var actionExecuted = false;

		// Act
		var returnedResult = result.Do(_ => actionExecuted = true);

		// Assert
		actionExecuted.ShouldBeFalse();
		returnedResult.ShouldBe(result);
	}

	[Fact]
	public async Task DoAsync_On_Success_Should_Execute_Action()
	{
		// Arrange
		var resultTask = Task.FromResult(Result.Success());
		var actionExecuted = false;

		// Act
		var returnedResult = await resultTask.DoAsync(() => actionExecuted = true);

		// Assert
		actionExecuted.ShouldBeTrue();
		returnedResult.IsSuccess.ShouldBeTrue();
	}

	[Fact]
	public async Task DoAsync_On_Failure_Should_Not_Execute_Action()
	{
		// Arrange
		var error = Error.Failure("Test.Error", "Test error");
		var resultTask = Task.FromResult(Result.Failure(error));
		var actionExecuted = false;

		// Act
		var returnedResult = await resultTask.DoAsync(() => actionExecuted = true);

		// Assert
		actionExecuted.ShouldBeFalse();
		returnedResult.IsFailure.ShouldBeTrue();
	}

	[Fact]
	public async Task DoAsync_With_Value_On_Success_Should_Execute_Action()
	{
		// Arrange
		var value = Faker.Random.Int();
		var resultTask = Task.FromResult(Result.Success(value));
		var capturedValue = 0;

		// Act
		var returnedResult = await resultTask.DoAsync(v => capturedValue = v);

		// Assert
		capturedValue.ShouldBe(value);
		returnedResult.Value.ShouldBe(value);
	}

	[Fact]
	public async Task DoAsync_With_Async_Action_On_Success_Should_Execute_Action()
	{
		// Arrange
		var value = Faker.Random.Int();
		var result = Result.Success(value);
		var capturedValue = 0;

		// Act
		var returnedResult = await result.DoAsync(async v =>
		{
			await Task.Delay(1);
			capturedValue = v;
		});

		// Assert
		capturedValue.ShouldBe(value);
		returnedResult.Value.ShouldBe(value);
	}
}
