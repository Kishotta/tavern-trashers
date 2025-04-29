namespace TavernTrashers.Api.Modules.Campaigns.Domain.Members;

public sealed class MemberNameChangedDomainEvent(Guid memberId, string firstName, string lastName) : DomainEvent
{
	public Guid MemberId { get; } = memberId;
	public string FirstName { get; } = firstName;
	public string LastName { get; } = lastName;
}