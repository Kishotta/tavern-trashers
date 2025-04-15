using TavernTrashers.Api.Common.Application.Messaging;

namespace TavernTrashers.Api.Modules.Campaigns.Application.Players.ChangePlayerName;

public sealed record ChangePlayerNameCommand(
	Guid PlayerId,
	string FirstName,
	string LastName) : ICommand;