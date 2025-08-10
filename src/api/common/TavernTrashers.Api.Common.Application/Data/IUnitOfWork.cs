namespace TavernTrashers.Api.Common.Application.Data;

public interface IUnitOfWork
{
	Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}