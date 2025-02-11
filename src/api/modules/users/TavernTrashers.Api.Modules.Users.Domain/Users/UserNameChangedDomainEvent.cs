using TavernTrashers.Api.Common.Domain.Entities;

namespace TavernTrashers.Api.Modules.Users.Domain.Users;

public sealed class UserNameChangedDomainEvent(
	Guid userId, 
	string firstName,
	string lastName) : DomainEvent
{
	public Guid UserId { get; } = userId;
	public string FirstName { get; } = firstName;
	public string LastName { get; } = lastName;
}