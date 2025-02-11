using Microsoft.EntityFrameworkCore;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Users.Domain.Users;
using TavernTrashers.Api.Modules.Users.Infrastructure.Database;

namespace TavernTrashers.Api.Modules.Users.Infrastructure.Users;

internal sealed class UserRepository(UsersDbContext db) : IUserRepository
{
    public async Task<Result<User>> GetAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await db
           .Users
           .SingleOrDefaultAsync(user => user.Id == userId, cancellationToken)
           .ToResultAsync(UserErrors.NotFound(userId));

    public async Task<Result<User>> GetReadOnlyAsync(Guid userId, CancellationToken cancellationToken = default) => 
        await db
           .Users
           .AsNoTracking()
           .SingleOrDefaultAsync(user => user.Id == userId, cancellationToken)
           .ToResultAsync(UserErrors.NotFound(userId));

    public void Insert(User user)
    {
        foreach (var role in user.Roles)
            db.Attach(role);

        db.Users.Add(user);
    }
}