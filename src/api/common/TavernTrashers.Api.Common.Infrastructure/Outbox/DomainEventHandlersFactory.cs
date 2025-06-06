using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using TavernTrashers.Api.Common.Application.Messaging;

namespace TavernTrashers.Api.Common.Infrastructure.Outbox;

public static class DomainEventHandlersFactory
{
    private static readonly ConcurrentDictionary<string, Type[]> HandlersDictionary = new();

    public static IEnumerable<IDomainEventHandler> GetHandlers(
        Type domainEventType,
        IServiceProvider serviceProvider,
        Assembly assembly)
    {
        var domainEventHandlerTypes = HandlersDictionary.GetOrAdd(
            $"{assembly.GetName().Name}{domainEventType.Name}",
            _ => assembly
               .GetTypes()
               .Where(handlerType => handlerType
                   .IsAssignableTo(typeof(IDomainEventHandler<>).MakeGenericType(domainEventType)))
               .ToArray());

        return domainEventHandlerTypes
            .Select(serviceProvider.GetRequiredService)
            .Select(domainEventHandler => (domainEventHandler as IDomainEventHandler)!)
            .ToList();
    }
    
}