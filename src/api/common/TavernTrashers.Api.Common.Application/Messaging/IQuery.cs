using MediatR;
using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Common.Application.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;