using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Modules.Users.Domain.Users;

public interface IUserRepository
{
	Task<Result<User>> GetAsync(Guid userId, CancellationToken cancellationToken = default);
	Task<Result<User>> GetReadOnlyAsync(Guid userId, CancellationToken cancellationToken = default);

	void Insert(User user);
}