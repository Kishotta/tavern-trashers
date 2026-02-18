namespace TavernTrashers.Api.Modules.Users.Application.Abstractions.Identity;

/// <summary>
/// Abstraction for KeyCloak admin API client operations.
/// </summary>
public interface IKeyCloakClient
{
	/// <summary>
	/// Registers a new user in KeyCloak.
	/// </summary>
	/// <param name="user">The user representation to register.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>The identity ID of the newly created user.</returns>
	Task<string> RegisterUserAsync(UserRepresentation user, CancellationToken cancellationToken = default);
}
