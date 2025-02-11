using TavernTrashers.Api.Common.Application.Messaging;

namespace TavernTrashers.Api.Modules.Users.Application.Users.ChangeUserName;

public sealed record ChangeUserNameCommand(
	Guid UserId, 
	string FirstName,
	string LastName) : ICommand<UserResponse>;