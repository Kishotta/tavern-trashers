using TavernTrashers.Api.Common.Application.Messaging;

namespace TavernTrashers.Api.Common.Application.Caching;

public interface ICacheInvalidationCommand : ICommand
{
	string[] CacheKeys { get; }
}

public interface ICacheInvalidationCommand<TResponse> : ICommand<TResponse>
{
	string[] CacheKeys { get; }
}