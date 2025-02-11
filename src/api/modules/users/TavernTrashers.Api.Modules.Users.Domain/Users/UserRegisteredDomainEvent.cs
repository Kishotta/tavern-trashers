using TavernTrashers.Api.Common.Domain.Entities;

namespace TavernTrashers.Api.Modules.Users.Domain.Users;

public sealed class UserRegisteredDomainEvent(Guid userId) : DomainEvent
{
	public Guid UserId { get; } = userId;
}