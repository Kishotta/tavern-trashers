using Microsoft.AspNetCore.Routing;

namespace TavernTrashers.Api.Common.Presentation;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}