using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Modules.Users.Application.Abstractions.Identity;

public interface IIdentityProviderService
{
	Task<Result<string>> RegisterUserAsync(UserModel user, CancellationToken cancellationToken = default);
}