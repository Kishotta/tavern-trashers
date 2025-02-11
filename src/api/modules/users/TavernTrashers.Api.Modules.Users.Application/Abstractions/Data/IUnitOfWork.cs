namespace TavernTrashers.Api.Modules.Users.Application.Abstractions.Data;

public interface IUnitOfWork
{
	Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}