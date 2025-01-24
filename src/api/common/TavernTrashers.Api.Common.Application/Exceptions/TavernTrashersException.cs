using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Common.Application.Exceptions;

public sealed class TavernTrashersException(
    string requestName,
    Error? error = default,
    Exception? innerException = default)
    : Exception("Application exception", innerException)
{
    public string RequestName { get; } = requestName;

    public Error? Error { get; } = error;
}
