namespace TavernTrashers.Api.Modules.Campaigns.Domain.Members;

public sealed class MemberCreatedDomainEvent(Guid memberId) : DomainEvent
{
	public Guid MemberId { get; } = memberId;
}