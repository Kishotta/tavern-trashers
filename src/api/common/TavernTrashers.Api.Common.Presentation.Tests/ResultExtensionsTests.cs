using Shouldly;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Presentation.Endpoints;

namespace TavernTrashers.Api.Common.Presentation.Tests;

public class ResultExtensionsTests
{
	[Fact]
	public void Ok_On_Success_Result_Should_Return_Result()
	{
		// Arrange
		var result = Result.Success();

		// Act
		var httpResult = result.Ok();

		// Assert
		httpResult.ShouldNotBeNull();
	}

	[Fact]
	public void Ok_On_Failure_Result_Should_Return_Result()
	{
		// Arrange
		var error = Error.Failure("Test.Error", "Test error");
		var result = Result.Failure(error);

		// Act
		var httpResult = result.Ok();

		// Assert
		httpResult.ShouldNotBeNull();
	}

	[Fact]
	public void Ok_With_Value_On_Success_Should_Return_Result()
	{
		// Arrange
		var value = 42;
		var result = Result.Success(value);

		// Act
		var httpResult = result.Ok();

		// Assert
		httpResult.ShouldNotBeNull();
	}

	[Fact]
	public void Ok_With_Value_On_Failure_Should_Return_Result()
	{
		// Arrange
		var error = Error.Failure("Test.Error", "Test error");
		var result = Result.Failure<int>(error);

		// Act
		var httpResult = result.Ok();

		// Assert
		httpResult.ShouldNotBeNull();
	}

	[Fact]
	public void Created_On_Success_Should_Return_Result()
	{
		// Arrange
		var value = 42;
		var result = Result.Success(value);
		var uri = new Uri("https://example.com/resource/42");

		// Act
		var httpResult = result.Created(_ => uri);

		// Assert
		httpResult.ShouldNotBeNull();
	}

	[Fact]
	public void Created_On_Failure_Should_Return_Result()
	{
		// Arrange
		var error = Error.Failure("Test.Error", "Test error");
		var result = Result.Failure<int>(error);
		var uri = new Uri("https://example.com/resource/42");

		// Act
		var httpResult = result.Created(_ => uri);

		// Assert
		httpResult.ShouldNotBeNull();
	}

	[Fact]
	public async Task OkAsync_On_Success_Should_Return_Result()
	{
		// Arrange
		var resultTask = Task.FromResult(Result.Success());

		// Act
		var httpResult = await resultTask.OkAsync();

		// Assert
		httpResult.ShouldNotBeNull();
	}

	[Fact]
	public async Task OkAsync_With_Value_On_Success_Should_Return_Result()
	{
		// Arrange
		var value = 42;
		var resultTask = Task.FromResult(Result.Success(value));

		// Act
		var httpResult = await resultTask.OkAsync();

		// Assert
		httpResult.ShouldNotBeNull();
	}

	[Fact]
	public async Task CreatedAsync_On_Success_Should_Return_Result()
	{
		// Arrange
		var value = 42;
		var resultTask = Task.FromResult(Result.Success(value));
		var uri = new Uri("https://example.com/resource/42");

		// Act
		var httpResult = await resultTask.CreatedAsync(_ => uri);

		// Assert
		httpResult.ShouldNotBeNull();
	}
}
