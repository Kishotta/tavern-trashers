using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Campaigns.Application.Abstractions.Data;
using TavernTrashers.Api.Modules.Campaigns.Domain.Players;

namespace TavernTrashers.Api.Modules.Campaigns.Application.Players;

public sealed record ChangePlayerNameCommand(
	Guid PlayerId,
	string FirstName,
	string LastName) : ICommand;

internal sealed class ChangePlayerNameCommandHandler(IPlayerRepository playerRepository, IUnitOfWork unitOfWork)
	: ICommandHandler<ChangePlayerNameCommand>
{
	public async Task<Result> Handle(ChangePlayerNameCommand command, CancellationToken cancellationToken) =>
		await playerRepository
		   .GetAsync(command.PlayerId, cancellationToken)
		   .ThenAsync(player => player.ChangeName(command.FirstName, command.LastName))
		   .DoAsync(_ => unitOfWork.SaveChangesAsync(cancellationToken));
}