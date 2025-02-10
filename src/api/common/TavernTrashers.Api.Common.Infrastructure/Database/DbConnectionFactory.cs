using System.Data.Common;
using Npgsql;
using TavernTrashers.Api.Common.Application.Data;

namespace TavernTrashers.Api.Common.Infrastructure.Database;

internal sealed class DbConnectionFactory(NpgsqlDataSource dataSource) : IDbConnectionFactory
{
	public async ValueTask<DbConnection> OpenConnectionAsync()
	{
		return await dataSource.OpenConnectionAsync();
	}
}