using Shouldly;
using TavernTrashers.Api.Common.Infrastructure.Tests.Fixtures;

namespace TavernTrashers.Api.Common.Infrastructure.Tests.Integration;

/// <summary>
/// Example integration test demonstrating how to use the PostgresTestFixture.
/// This test validates that the test infrastructure is working correctly.
/// </summary>
public class PostgresTestFixtureIntegrationTests : IClassFixture<PostgresTestFixture>
{
	private readonly PostgresTestFixture _fixture;

	public PostgresTestFixtureIntegrationTests(PostgresTestFixture fixture)
	{
		_fixture = fixture;
	}

	[Fact]
	public void Fixture_Initialized_ConnectionStringIsNotEmpty()
	{
		// Assert
		_fixture.ConnectionString.ShouldNotBeNullOrWhiteSpace();
		_fixture.ConnectionString.ShouldContain("tavern_trashers_test");
	}

	[Fact]
	public async Task PostgresContainer_IsRunning_CanExecuteQuery()
	{
		// Arrange
		await using var connection = new Npgsql.NpgsqlConnection(_fixture.ConnectionString);
		await connection.OpenAsync();

		// Act
		await using var command = connection.CreateCommand();
		command.CommandText = "SELECT 1 + 1 AS result";
		var result = await command.ExecuteScalarAsync();

		// Assert
		result.ShouldNotBeNull();
		Convert.ToInt32(result).ShouldBe(2);
	}

	[Fact]
	public async Task PostgresContainer_CanCreateAndQueryTable()
	{
		// Arrange
		await using var connection = new Npgsql.NpgsqlConnection(_fixture.ConnectionString);
		await connection.OpenAsync();

		// Create table
		await using var createCommand = connection.CreateCommand();
		createCommand.CommandText = @"
			CREATE TEMP TABLE test_users (
				id SERIAL PRIMARY KEY,
				name VARCHAR(100) NOT NULL,
				email VARCHAR(255) NOT NULL UNIQUE
			)";
		await createCommand.ExecuteNonQueryAsync();

		// Act - Insert data
		await using var insertCommand = connection.CreateCommand();
		insertCommand.CommandText = @"
			INSERT INTO test_users (name, email) 
			VALUES (@name, @email)
			RETURNING id";
		insertCommand.Parameters.AddWithValue("name", "Test User");
		insertCommand.Parameters.AddWithValue("email", "test@example.com");
		var insertedId = await insertCommand.ExecuteScalarAsync();

		// Query data
		await using var selectCommand = connection.CreateCommand();
		selectCommand.CommandText = "SELECT name, email FROM test_users WHERE id = @id";
		selectCommand.Parameters.AddWithValue("id", insertedId!);
		
		await using var reader = await selectCommand.ExecuteReaderAsync();
		await reader.ReadAsync();

		// Assert
		reader.GetString(0).ShouldBe("Test User");
		reader.GetString(1).ShouldBe("test@example.com");
	}
}
