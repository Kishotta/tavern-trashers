using System.Data.Common;

namespace TavernTrashers.Api.Common.Application.Data;

public interface IDbConnectionFactory
{
    ValueTask<DbConnection> OpenConnectionAsync();
}