using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Modules.Campaigns.Domain.Members;

public static class MemberErrors
{
	public static Error NotFound(Guid memberId) =>
		Error.NotFound(
			"Member.NotFound",
			$"The member with identifier {memberId} was not found");
}