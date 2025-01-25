using Microsoft.Extensions.DependencyInjection;
using TavernTrashers.Api.Common.Application.Clock;

namespace TavernTrashers.Api.Common.Infrastructure.Clock;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}

internal static class DateTimeExtensions
{
    internal static IServiceCollection AddDateTimeProvider(this IServiceCollection services)
    {
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        
        return services;
    }
}