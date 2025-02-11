using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Modules.Users.Application.Abstractions.Identity;

public static class IdentityProviderErrors
{
	public static readonly Error EmailIsNotUnique = 
		Error.Conflict(
			"Identity.EmailIsNotUnique",
			"The specified email is not unique");
}