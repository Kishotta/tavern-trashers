using System.Net;
using Microsoft.Extensions.Logging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Modules.Users.Application.Abstractions.Identity;

namespace TavernTrashers.Api.Modules.Users.Infrastructure.Identity;

internal sealed class KeyCloakIdentityProviderService(
    IKeyCloakClient keyCloakClient,
    KeyCloakTokenClient keyCloakTokenClient,
    ILogger<KeyCloakIdentityProviderService> logger)
    : IIdentityProviderService
{ 
    private const string PasswordCredentialType = "Password";
    
    public async Task<Result<string>> RegisterUserAsync(UserModel user, CancellationToken cancellationToken = default)
    {
        var userRepresentation = new UserRepresentation(
            user.Email,
            user.Email,
            user.FirstName,
            user.LastName,
            true,
            true,
            [
                new CredentialRepresentation(PasswordCredentialType, user.Password, false)
            ]);

        try
        {
            return await keyCloakClient.RegisterUserAsync(userRepresentation, cancellationToken);
        }
        catch (HttpRequestException exception) when (exception.StatusCode == HttpStatusCode.Conflict)
        {
            logger.LogError(exception, "User registration failed");

            return IdentityProviderErrors.EmailIsNotUnique;
        }
    }
    
    public async Task<Result<AuthToken>> GetUserAuthTokenAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            return await keyCloakTokenClient.GetUserAuthToken(email, password, cancellationToken);
        }
        catch (HttpRequestException exception) when (exception.StatusCode == HttpStatusCode.BadRequest)
        {
            logger.LogError(exception, "User authentication failed");

            return IdentityProviderErrors.InvalidCredentials;
        }
    }
}