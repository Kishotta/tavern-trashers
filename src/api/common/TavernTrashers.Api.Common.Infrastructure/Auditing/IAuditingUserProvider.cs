namespace TavernTrashers.Api.Common.Infrastructure.Auditing;

public interface IAuditingUserProvider
{
    string GetUserId();
}