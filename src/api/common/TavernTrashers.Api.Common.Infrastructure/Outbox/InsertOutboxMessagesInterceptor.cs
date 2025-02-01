using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Newtonsoft.Json;
using TavernTrashers.Api.Common.Domain.Entities;
using TavernTrashers.Api.Common.Infrastructure.Serialization;

namespace TavernTrashers.Api.Common.Infrastructure.Outbox;

public sealed class InsertOutboxMessagesInterceptor : SaveChangesInterceptor
{
	public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
		DbContextEventData eventData,
		InterceptionResult<int> result,
		CancellationToken cancellationToken = default)
	{
		if (eventData.Context is not null)
			InsertOutboxMessages(eventData.Context);

		return await base.SavingChangesAsync(eventData, result, cancellationToken);
	}

	private static void InsertOutboxMessages(DbContext context)
	{
		var outboxMessages = context
		   .ChangeTracker
		   .Entries<EntityBase>()
		   .Select(entry => entry.Entity)
		   .SelectMany(entity =>
			{
				var domainEvents = entity.GetDomainEvents();
				entity.ClearDomainEvents();
				return domainEvents;
			})
		   .Select(domainEvent => new OutboxMessage
			{
				Id            = domainEvent.Id,
				Type          = domainEvent.GetType().Name,
				Content       = JsonConvert.SerializeObject(domainEvent, SerializerSettings.Instance),
				OccurredAtUtc = domainEvent.OccurredAtUtc,
			})
		   .ToList();
		
		context.Set<OutboxMessage>().AddRange(outboxMessages);
	}
}