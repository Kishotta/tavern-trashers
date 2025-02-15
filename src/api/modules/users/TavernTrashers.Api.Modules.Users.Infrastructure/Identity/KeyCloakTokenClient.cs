using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TavernTrashers.Api.Modules.Users.Infrastructure.Identity;

internal sealed class KeyCloakTokenClient(IOptions<KeyCloakOptions> options, HttpClient httpClient)
{
	private readonly KeyCloakOptions _options = options.Value;
    
	internal async Task<string> GetAccessToken(CancellationToken cancellationToken)
	{
		var authRequestParameters = new KeyValuePair<string, string>[]
		{
			new("client_id", _options.ConfidentialClientId),
			new("client_secret", _options.ConfidentialClientSecret),
			new("scope", "openid"),
			new("grant_type", "client_credentials")
		};

		using var authRequestContent = new FormUrlEncodedContent(authRequestParameters);
        
		using var authRequest = new HttpRequestMessage(HttpMethod.Post, _options.TokenUrl);
		authRequest.Content = authRequestContent;
        
		using var authorizationResponse = await httpClient.SendAsync(authRequest, cancellationToken);

		authorizationResponse.EnsureSuccessStatusCode();
		
		var authToken = (await authorizationResponse.Content.ReadFromJsonAsync<AuthToken>(cancellationToken))!;
		
		return authToken.AccessToken;
	}
    
	internal sealed class AuthToken
	{
		[JsonPropertyName("access_token")] public string AccessToken { get; init; } = string.Empty;
	}
}