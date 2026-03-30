using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using TavernTrashers.Api.Common.Presentation.Authentication;
using TavernTrashers.Api.Common.Presentation.Endpoints;
using TavernTrashers.Api.Modules.Campaigns.Application.Campaigns;

namespace TavernTrashers.Api.Modules.Campaigns.Presentation.Campaigns;

public class CreateCampaign : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("/campaigns", async (
                    CreateCampaignRequest request,
                    ClaimsPrincipal claims,
                    ISender sender) =>
                await sender
                   .Send(new CreateCampaignCommand(request.Title, request.Description, claims.GetUserId()))
                   .OkAsync())
           .RequireAuthorization()
           .WithName(nameof(CreateCampaign))
           .WithTags(Tags.Campaigns)
           .WithSummary("Create Campaign")
           .WithDescription("Create a new campaign.")
           .Accepts<CreateCampaignRequest>("application/json")
           .Produces<CampaignResponse>(StatusCodes.Status200OK)
           .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
           .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized);

    internal sealed record CreateCampaignRequest(string Title, string Description);
}
