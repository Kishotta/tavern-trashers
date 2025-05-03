namespace TavernTrashers.Api.Modules.Dice.Application.Abstractions.Data;

public interface IUnitOfWork
{
	Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}