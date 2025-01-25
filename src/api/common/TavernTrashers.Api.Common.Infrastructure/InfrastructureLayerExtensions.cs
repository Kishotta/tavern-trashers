using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TavernTrashers.Api.Common.Application.Clock;
using TavernTrashers.Api.Common.Infrastructure.Auditing;
using TavernTrashers.Api.Common.Infrastructure.Caching;
using TavernTrashers.Api.Common.Infrastructure.Clock;
using TavernTrashers.Api.Common.Infrastructure.Outbox;

namespace TavernTrashers.Api.Common.Infrastructure;

public static class InfrastructureLayerExtensions
{
	public static IServiceCollection ConfigureInfrastructureLayer(this IServiceCollection services)
	{
		services
		   .AddAuditing()
		   .AddCache()
		   .AddDateTimeProvider()
		   .AddOutbox();

		return services;
	}
}