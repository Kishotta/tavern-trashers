using Shouldly;
using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Common.Domain.Tests;

public class ErrorTests : TestBase
{
	[Fact]
	public void Error_None_Should_Have_Empty_Values()
	{
		// Arrange & Act
		var error = Error.None;

		// Assert
		error.Code.ShouldBeEmpty();
		error.Description.ShouldBeEmpty();
		error.Type.ShouldBe(ErrorType.Failure);
	}

	[Fact]
	public void Error_NullValue_Should_Have_Correct_Values()
	{
		// Arrange & Act
		var error = Error.NullValue;

		// Assert
		error.Code.ShouldBe("General.Null");
		error.Description.ShouldBe("Null value was provided");
		error.Type.ShouldBe(ErrorType.Failure);
	}

	[Fact]
	public void Failure_Should_Create_Error_With_Failure_Type()
	{
		// Arrange
		var code = Faker.Random.String2(10);
		var description = Faker.Random.String2(20);

		// Act
		var error = Error.Failure(code, description);

		// Assert
		error.Code.ShouldBe(code);
		error.Description.ShouldBe(description);
		error.Type.ShouldBe(ErrorType.Failure);
	}

	[Fact]
	public void Validation_Should_Create_Error_With_Validation_Type()
	{
		// Arrange
		var code = Faker.Random.String2(10);
		var description = Faker.Random.String2(20);

		// Act
		var error = Error.Validation(code, description);

		// Assert
		error.Code.ShouldBe(code);
		error.Description.ShouldBe(description);
		error.Type.ShouldBe(ErrorType.Validation);
	}

	[Fact]
	public void Problem_Should_Create_Error_With_Problem_Type()
	{
		// Arrange
		var code = Faker.Random.String2(10);
		var description = Faker.Random.String2(20);

		// Act
		var error = Error.Problem(code, description);

		// Assert
		error.Code.ShouldBe(code);
		error.Description.ShouldBe(description);
		error.Type.ShouldBe(ErrorType.Problem);
	}

	[Fact]
	public void NotFound_Should_Create_Error_With_NotFound_Type()
	{
		// Arrange
		var code = Faker.Random.String2(10);
		var description = Faker.Random.String2(20);

		// Act
		var error = Error.NotFound(code, description);

		// Assert
		error.Code.ShouldBe(code);
		error.Description.ShouldBe(description);
		error.Type.ShouldBe(ErrorType.NotFound);
	}

	[Fact]
	public void Conflict_Should_Create_Error_With_Conflict_Type()
	{
		// Arrange
		var code = Faker.Random.String2(10);
		var description = Faker.Random.String2(20);

		// Act
		var error = Error.Conflict(code, description);

		// Assert
		error.Code.ShouldBe(code);
		error.Description.ShouldBe(description);
		error.Type.ShouldBe(ErrorType.Conflict);
	}

	[Fact]
	public void Authorization_Should_Create_Error_With_Authorization_Type()
	{
		// Arrange
		var code = Faker.Random.String2(10);
		var description = Faker.Random.String2(20);

		// Act
		var error = Error.Authorization(code, description);

		// Assert
		error.Code.ShouldBe(code);
		error.Description.ShouldBe(description);
		error.Type.ShouldBe(ErrorType.Authorization);
	}
}
