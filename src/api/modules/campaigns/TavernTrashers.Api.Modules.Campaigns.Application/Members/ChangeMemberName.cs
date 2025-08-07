using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Campaigns.Domain.Members;

namespace TavernTrashers.Api.Modules.Campaigns.Application.Members;

public sealed record ChangeMemberNameCommand(
	Guid MemberId,
	string FirstName,
	string LastName) : ICommand;

internal sealed class ChangeMemberNameCommandHandler(IMemberRepository memberRepository)
	: ICommandHandler<ChangeMemberNameCommand>
{
	public async Task<Result> Handle(ChangeMemberNameCommand command, CancellationToken cancellationToken) =>
		await memberRepository
		   .GetAsync(command.MemberId, cancellationToken)
		   .ThenAsync(player => player.ChangeName(command.FirstName, command.LastName));
}