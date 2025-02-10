using Microsoft.AspNetCore.Routing;

namespace TavernTrashers.Api.Common.Presentation.Endpoints;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}