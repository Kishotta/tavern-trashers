using TavernTrashers.Api.Common.Application.Clock;

namespace TavernTrashers.Api.Common.Infrastructure.Clock;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}