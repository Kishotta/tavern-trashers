using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;

namespace TavernTrashers.Api.Modules.Users.Infrastructure.Identity;

internal sealed class KeyCloakAuthDelegatingHandler(KeyCloakTokenClient tokenClient) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, 
        CancellationToken cancellationToken)
    {
        var authorizationToken = await tokenClient.GetAccessToken(cancellationToken);
        
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authorizationToken);
        
        var httpResponseMessage = await base.SendAsync(request, cancellationToken);
        
        httpResponseMessage.EnsureSuccessStatusCode();
        
        return httpResponseMessage;
    }
    
}