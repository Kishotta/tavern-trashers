using TavernTrashers.Api.Common.Domain.Auditing;
using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Modules.Campaigns.Domain.Members;

[Auditable]
public class Member : Entity
{
	private Member() { }
	public string FirstName { get; private set; } = string.Empty;
	public string LastName { get; private set; } = string.Empty;
	public string Email { get; private set; } = string.Empty;

	public static Result<Member> Create(
		Guid id,
		string firstName,
		string lastName,
		string email)
	{
		var player = new Member
		{
			Id        = id,
			FirstName = firstName,
			LastName  = lastName,
			Email     = email,
		};

		player.RaiseDomainEvent(new MemberCreatedDomainEvent(player.Id));

		return player;
	}

	public Result<Member> ChangeName(string firstName, string lastName)
	{
		if (FirstName == firstName && LastName == lastName) return this;

		FirstName = firstName;
		LastName  = lastName;

		RaiseDomainEvent(new MemberNameChangedDomainEvent(Id, firstName, lastName));

		return this;
	}
}