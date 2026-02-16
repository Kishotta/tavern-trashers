using Shouldly;
using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Common.Domain.Tests;

public class ResultTests : TestBase
{
	[Fact]
	public void Success_Result_Should_Have_IsSuccess_True()
	{
		// Arrange & Act
		var result = Result.Success();

		// Assert
		result.IsSuccess.ShouldBeTrue();
		result.IsFailure.ShouldBeFalse();
		result.Error.ShouldBe(Error.None);
	}

	[Fact]
	public void Success_Result_With_Value_Should_Have_IsSuccess_True()
	{
		// Arrange
		var value = Faker.Random.Int();

		// Act
		var result = Result.Success(value);

		// Assert
		result.IsSuccess.ShouldBeTrue();
		result.IsFailure.ShouldBeFalse();
		result.Value.ShouldBe(value);
		result.Error.ShouldBe(Error.None);
	}

	[Fact]
	public void Failure_Result_Should_Have_IsFailure_True()
	{
		// Arrange
		var error = Error.Failure("Test.Error", "Test error message");

		// Act
		var result = Result.Failure(error);

		// Assert
		result.IsFailure.ShouldBeTrue();
		result.IsSuccess.ShouldBeFalse();
		result.Error.ShouldBe(error);
	}

	[Fact]
	public void Failure_Result_With_Value_Should_Have_IsFailure_True()
	{
		// Arrange
		var error = Error.Failure("Test.Error", "Test error message");

		// Act
		var result = Result.Failure<int>(error);

		// Assert
		result.IsFailure.ShouldBeTrue();
		result.IsSuccess.ShouldBeFalse();
		result.Error.ShouldBe(error);
	}

	[Fact]
	public void Accessing_Value_On_Failed_Result_Should_Throw_InvalidOperationException()
	{
		// Arrange
		var error = Error.Failure("Test.Error", "Test error message");
		var result = Result.Failure<int>(error);

		// Act & Assert
		Should.Throw<InvalidOperationException>(() => result.Value);
	}

	[Fact]
	public void Implicit_Conversion_From_Value_To_Result_Should_Create_Success()
	{
		// Arrange
		var value = Faker.Random.Int();

		// Act
		Result<int> result = value;

		// Assert
		result.IsSuccess.ShouldBeTrue();
		result.Value.ShouldBe(value);
	}

	[Fact]
	public void Implicit_Conversion_From_Null_Value_To_Result_Should_Create_Failure()
	{
		// Arrange
		string? nullValue = null;

		// Act
		Result<string> result = nullValue!;

		// Assert
		result.IsFailure.ShouldBeTrue();
		result.Error.ShouldBe(Error.NullValue);
	}

	[Fact]
	public void Implicit_Conversion_From_Error_To_Result_Should_Create_Failure()
	{
		// Arrange
		var error = Error.Failure("Test.Error", "Test error message");

		// Act
		Result result = error;

		// Assert
		result.IsFailure.ShouldBeTrue();
		result.Error.ShouldBe(error);
	}

	[Fact]
	public void Implicit_Conversion_From_Error_To_Result_With_Value_Should_Create_Failure()
	{
		// Arrange
		var error = Error.Failure("Test.Error", "Test error message");

		// Act
		Result<int> result = error;

		// Assert
		result.IsFailure.ShouldBeTrue();
		result.Error.ShouldBe(error);
	}

	[Fact]
	public void ValidationFailure_Should_Create_Result_With_Validation_Error()
	{
		// Arrange
		var error = Error.Validation("Test.Validation", "Validation failed");

		// Act
		var result = Result<int>.ValidationFailure(error);

		// Assert
		result.IsFailure.ShouldBeTrue();
		result.Error.ShouldBe(error);
		result.Error.Type.ShouldBe(ErrorType.Validation);
	}

	[Fact]
	public void Creating_Success_Result_With_Error_Should_Throw_ArgumentException()
	{
		// Arrange
		var error = Error.Failure("Test.Error", "Test error message");

		// Act & Assert
		Should.Throw<ArgumentException>(() => new Result<int>(42, true, error));
	}

	[Fact]
	public void Creating_Failure_Result_Without_Error_Should_Throw_ArgumentException()
	{
		// Act & Assert
		Should.Throw<ArgumentException>(() => new Result<int>(42, false, Error.None));
	}
}
