using System.Text.Json.Serialization;

namespace TavernTrashers.Api.Modules.Users.Application.Abstractions.Identity;

public sealed class AuthToken
{
	[JsonPropertyName("access_token")] 
	public string AccessToken { get; init; } = string.Empty;
		
	[JsonPropertyName("token_type")]
	public string TokenType { get; init; } = string.Empty;
		
	[JsonPropertyName("refresh_token")]
	public string RefreshToken { get; init; } = string.Empty;
		
	[JsonPropertyName("expires_in")]
	public int ExpiresIn { get; init; }
		
	[JsonPropertyName("id_token")]
	public string IdToken { get; init; } = string.Empty;
}