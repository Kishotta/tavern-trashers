namespace TavernTrashers.Api.Modules.Users.Application.Abstractions.Identity;

/// <summary>
/// Represents a user in KeyCloak.
/// </summary>
public sealed record UserRepresentation(
	string Username,
	string Email,
	string FirstName,
	string LastName,
	bool EmailVerified,
	bool Enabled,
	CredentialRepresentation[] Credentials);
