using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Modules.Users.Application.Abstractions.Identity;

namespace TavernTrashers.Api.Modules.Users.Application.Users.RegisterUser;

public sealed record RegisterUserCommand(
	string Email,
	string Password,
	string FirstName,
	string LastName) : ICommand<AuthToken>;