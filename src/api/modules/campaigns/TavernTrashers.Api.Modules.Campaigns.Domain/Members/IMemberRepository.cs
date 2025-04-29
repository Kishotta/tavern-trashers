using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Modules.Campaigns.Domain.Members;

public interface IMemberRepository
{
	Task<Result<Member>> GetAsync(Guid memberId, CancellationToken cancellationToken = default);
	void Add(Member member);
}