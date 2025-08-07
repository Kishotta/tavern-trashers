using TavernTrashers.Api.Common.Domain.Auditing;
using TavernTrashers.Api.Common.Domain.Entities;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Modules.Users.Domain.Permissions;

namespace TavernTrashers.Api.Modules.Users.Domain.Users;

[Auditable]
public sealed class User : Entity
{
	public string Email { get; private set; } = string.Empty;
	public string FirstName { get; private set; } = string.Empty;
	public string LastName { get; private set; } = string.Empty;
	public string IdentityId { get; private set; } = string.Empty;
	
	public IReadOnlyCollection<Role> Roles => _roles.AsReadOnly();
	private readonly List<Role> _roles = [];
    
	private User() { }
    
	public static Result<User> Create(
		string email,
		string firstName,
		string lastName,
		string identityId)
	{
		var user = new User
		{
			Id         = Guid.NewGuid(),
			Email      = email,
			FirstName  = firstName,
			LastName   = lastName,
			IdentityId = identityId
		};
        
		user._roles.Add(Role.Player);
        
		user.RaiseDomainEvent(new UserRegisteredDomainEvent(user.Id));

		return user;
	}
    
	public void ChangeName(string firstName, string lastName)
	{
		if (FirstName == firstName && LastName == lastName) return;
        
		FirstName = firstName;
		LastName  = lastName;
        
		RaiseDomainEvent(new UserNameChangedDomainEvent(Id, FirstName, LastName));
	}
}