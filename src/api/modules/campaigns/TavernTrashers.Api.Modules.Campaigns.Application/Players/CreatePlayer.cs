using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Campaigns.Application.Abstractions.Data;
using TavernTrashers.Api.Modules.Campaigns.Domain.Players;

namespace TavernTrashers.Api.Modules.Campaigns.Application.Players;

public sealed record CreatePlayerCommand(
	Guid PlayerId,
	string FirstName,
	string LastName,
	string Email) : ICommand;

internal sealed class CreatePlayerCommandHandler(IPlayerRepository playerRepository, IUnitOfWork unitOfWork)
	: ICommandHandler<CreatePlayerCommand>
{
	public async Task<Result> Handle(CreatePlayerCommand command, CancellationToken cancellationToken) =>
		await Player
		   .Create(command.PlayerId, command.FirstName, command.LastName, command.Email)
		   .Do(playerRepository.Add)
		   .DoAsync(_ => unitOfWork.SaveChangesAsync(cancellationToken));
}