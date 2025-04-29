using Microsoft.EntityFrameworkCore;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Campaigns.Domain.Members;
using TavernTrashers.Api.Modules.Campaigns.Infrastructure.Database;

namespace TavernTrashers.Api.Modules.Campaigns.Infrastructure.Members;

public class MemberRepository(CampaignsDbContext dbContext) : IMemberRepository
{
	public async Task<Result<Member>> GetAsync(Guid memberId, CancellationToken cancellationToken = default) =>
		await dbContext
		   .Members
		   .SingleOrDefaultAsync(player => player.Id == memberId, cancellationToken)
		   .ToResultAsync(MemberErrors.NotFound(memberId));

	public void Add(Member member) => dbContext.Members.Add(member);
}