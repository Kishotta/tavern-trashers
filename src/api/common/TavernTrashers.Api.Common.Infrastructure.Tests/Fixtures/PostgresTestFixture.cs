using Testcontainers.PostgreSql;

namespace TavernTrashers.Api.Common.Infrastructure.Tests.Fixtures;

/// <summary>
/// Test fixture that provides a PostgreSQL database for integration tests using Testcontainers.
/// Shared across test classes using IClassFixture.
/// </summary>
public sealed class PostgresTestFixture : IAsyncLifetime
{
	private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
		.WithImage("postgres:16-alpine")
		.WithDatabase("tavern_trashers_test")
		.WithUsername("test_user")
		.WithPassword("test_password")
		.Build();

	/// <summary>
	/// Gets the connection string for the PostgreSQL test database.
	/// </summary>
	public string ConnectionString { get; private set; } = string.Empty;

	/// <summary>
	/// Starts the PostgreSQL container before tests run.
	/// </summary>
	public async Task InitializeAsync()
	{
		await _postgres.StartAsync();
		ConnectionString = _postgres.GetConnectionString();
	}

	/// <summary>
	/// Stops and disposes of the PostgreSQL container after tests complete.
	/// </summary>
	public async Task DisposeAsync()
	{
		await _postgres.StopAsync();
		await _postgres.DisposeAsync();
	}
}
