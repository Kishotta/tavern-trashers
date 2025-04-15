using TavernTrashers.Api.Common.Application.Messaging;

namespace TavernTrashers.Api.Modules.Campaigns.Application.Players.CreatePlayer;

public sealed record CreatePlayerCommand(
	Guid PlayerId,
	string FirstName,
	string LastName,
	string Email) : ICommand;