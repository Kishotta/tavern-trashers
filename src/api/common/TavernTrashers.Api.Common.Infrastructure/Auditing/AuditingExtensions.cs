using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TavernTrashers.Api.Common.Domain.Auditing;
using TavernTrashers.Api.Common.Infrastructure.Authentication;

namespace TavernTrashers.Api.Common.Infrastructure.Auditing;

internal static class AuditingExtensions
{
    internal static IServiceCollection AddAuditing(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        
        services.TryAddSingleton<IAuditingUserProvider, JwtAuditingUserProvider>();
        
        services.TryAddSingleton<WriteAuditLogInterceptor>();
        
        return services;
    }
    
    internal static bool ShouldBeAudited(this EntityEntry entry) =>
        entry.State != EntityState.Detached && 
        entry.State != EntityState.Unchanged &&
        entry.Entity is not Audit &&
        entry.IsAuditable();

    private static bool IsAuditable(this EntityEntry entityEntry)
    {
        var entityAuditableAttribute = Attribute.GetCustomAttribute(
            entityEntry.Entity.GetType(), 
            typeof(AuditableAttribute)) as AuditableAttribute;

        return entityAuditableAttribute is not null;
    }

    internal static bool IsAuditable(this PropertyEntry propertyEntry)
    {
        var entityType = propertyEntry.EntityEntry.Entity.GetType();
        var propertyInfo = entityType.GetProperty(propertyEntry.Metadata.Name)!;
        var propertyAuditingDisabled = Attribute.IsDefined(propertyInfo, typeof(NotAuditableAttribute));

        return IsAuditable(propertyEntry.EntityEntry) && !propertyAuditingDisabled;
    }
}