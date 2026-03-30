using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using TavernTrashers.Api.Common.Presentation.Endpoints;
using TavernTrashers.Api.Modules.Campaigns.Application.Campaigns;

namespace TavernTrashers.Api.Modules.Campaigns.Presentation.Campaigns;

public class UpdateCampaign : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPut("/campaigns/{id:guid}", async (Guid id, UpdateCampaignRequest request, ISender sender) =>
                await sender
                   .Send(new UpdateCampaignCommand(id, request.Title, request.Description))
                   .OkAsync())
           .RequireAuthorization()
           .WithName(nameof(UpdateCampaign))
           .WithTags(Tags.Campaigns)
           .WithSummary("Update Campaign")
           .Accepts<UpdateCampaignRequest>("application/json")
           .Produces<CampaignResponse>(StatusCodes.Status200OK)
           .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
           .Produces<ProblemDetails>(StatusCodes.Status404NotFound);

    internal sealed record UpdateCampaignRequest(string Title, string Description);
}
