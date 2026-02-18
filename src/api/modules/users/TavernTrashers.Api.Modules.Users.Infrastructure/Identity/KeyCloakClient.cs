using System.Net.Http.Json;
using TavernTrashers.Api.Modules.Users.Application.Abstractions.Identity;

namespace TavernTrashers.Api.Modules.Users.Infrastructure.Identity;

internal sealed class KeyCloakClient(HttpClient httpClient) : IKeyCloakClient
{
    public async Task<string> RegisterUserAsync(
        UserRepresentation user, 
        CancellationToken cancellationToken = default)
    {
        var httpResponseMessage = await httpClient.PostAsJsonAsync(
            "users",
            user,
            cancellationToken);

        httpResponseMessage.EnsureSuccessStatusCode();

        return ExtractIdentityIdFromLocationHeader(httpResponseMessage);
    }

    private static string ExtractIdentityIdFromLocationHeader(HttpResponseMessage httpResponseMessage)
    {
        const string usersSegmentName = "users/";
            
        var locationHeader = httpResponseMessage.Headers.Location?.PathAndQuery;
        if (locationHeader is null)
            throw new InvalidOperationException("Location header is null");

        var userSegmentValueIndex = locationHeader.IndexOf(usersSegmentName, StringComparison.InvariantCultureIgnoreCase);

        var identityId = locationHeader[(userSegmentValueIndex + usersSegmentName.Length)..];

        return identityId;
    }
}