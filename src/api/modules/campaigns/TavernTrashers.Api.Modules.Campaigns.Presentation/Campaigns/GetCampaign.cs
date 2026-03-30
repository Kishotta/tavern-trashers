using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using TavernTrashers.Api.Common.Presentation.Endpoints;
using TavernTrashers.Api.Modules.Campaigns.Application.Campaigns;

namespace TavernTrashers.Api.Modules.Campaigns.Presentation.Campaigns;

public class GetCampaign : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/campaigns/{id:guid}", async (Guid id, ISender sender) =>
                await sender
                   .Send(new GetCampaignQuery(id))
                   .OkAsync())
           .RequireAuthorization()
           .WithName(nameof(GetCampaign))
           .WithTags(Tags.Campaigns)
           .WithSummary("Get Campaign")
           .WithDescription("Get a campaign by ID.")
           .Produces<CampaignResponse>(StatusCodes.Status200OK)
           .Produces<ProblemDetails>(StatusCodes.Status404NotFound);
}
