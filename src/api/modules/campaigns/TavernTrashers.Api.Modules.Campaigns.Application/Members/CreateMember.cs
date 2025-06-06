using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Campaigns.Application.Abstractions.Data;
using TavernTrashers.Api.Modules.Campaigns.Domain.Members;

namespace TavernTrashers.Api.Modules.Campaigns.Application.Members;

public sealed record CreateMemberCommand(
	Guid MemberId,
	string FirstName,
	string LastName,
	string Email) : ICommand;

internal sealed class CreateMemberCommandHandler(IMemberRepository memberRepository, IUnitOfWork unitOfWork)
	: ICommandHandler<CreateMemberCommand>
{
	public async Task<Result> Handle(CreateMemberCommand command, CancellationToken cancellationToken) =>
		await Member
		   .Create(command.MemberId, command.FirstName, command.LastName, command.Email)
		   .Do(memberRepository.Add)
		   .DoAsync(_ => unitOfWork.SaveChangesAsync(cancellationToken));
}