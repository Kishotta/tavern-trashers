namespace TavernTrashers.Api.Common.Application.Data;

public interface IUnitOfWorkBase
{
	Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}