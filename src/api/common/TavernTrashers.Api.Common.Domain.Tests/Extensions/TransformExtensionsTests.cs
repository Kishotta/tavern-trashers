using Shouldly;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;

namespace TavernTrashers.Api.Common.Domain.Tests.Extensions;

public class TransformExtensionsTests : TestBase
{
	[Fact]
	public void Transform_On_Success_Should_Map_Value()
	{
		// Arrange
		var value = Faker.Random.Int();
		var result = Result.Success(value);

		// Act
		var transformedResult = result.Transform(v => v.ToString());

		// Assert
		transformedResult.IsSuccess.ShouldBeTrue();
		transformedResult.Value.ShouldBe(value.ToString());
	}

	[Fact]
	public void Transform_On_Failure_Should_Return_Error()
	{
		// Arrange
		var error = Error.Failure("Test.Error", "Test error");
		var result = Result.Failure<int>(error);

		// Act
		var transformedResult = result.Transform(v => v.ToString());

		// Assert
		transformedResult.IsFailure.ShouldBeTrue();
		transformedResult.Error.ShouldBe(error);
	}

	[Fact]
	public void Transform_With_Result_Mapper_On_Success_Should_Return_Mapped_Result()
	{
		// Arrange
		var value = Faker.Random.Int();
		var result = Result.Success(value);
		var expectedString = value.ToString();

		// Act
		var transformedResult = result.Transform(v => Result.Success(v.ToString()));

		// Assert
		transformedResult.IsSuccess.ShouldBeTrue();
		transformedResult.Value.ShouldBe(expectedString);
	}

	[Fact]
	public void Transform_With_Result_Mapper_On_Success_Can_Return_Failure()
	{
		// Arrange
		var value = Faker.Random.Int();
		var result = Result.Success(value);
		var error = Error.Failure("Transform.Error", "Transform failed");

		// Act
		var transformedResult = result.Transform<int, string>(_ => error);

		// Assert
		transformedResult.IsFailure.ShouldBeTrue();
		transformedResult.Error.ShouldBe(error);
	}

	[Fact]
	public async Task TransformAsync_With_Task_Value_Should_Map_Value()
	{
		// Arrange
		var value = Faker.Random.Int();
		var taskValue = Task.FromResult(value);

		// Act
		var transformedValue = await TransformExtensions.TransformAsync(taskValue, v => v.ToString());

		// Assert
		transformedValue.ShouldBe(value.ToString());
	}
}
