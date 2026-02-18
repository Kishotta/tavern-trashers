namespace TavernTrashers.Api.Modules.Users.Application.Abstractions.Identity;

/// <summary>
/// Represents a credential in KeyCloak.
/// </summary>
public sealed record CredentialRepresentation(
	string Type,
	string Value,
	bool Temporary);
