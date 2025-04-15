using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using TavernTrashers.Api.Common.Presentation.Authentication;
using TavernTrashers.Api.Common.Presentation.Endpoints;
using TavernTrashers.Api.Modules.Campaigns.Application.Campaigns;
using TavernTrashers.Api.Modules.Campaigns.Application.Campaigns.CreateCampaign;

namespace TavernTrashers.Api.Modules.Campaigns.Presentation.Campaigns;

public class CreateCampaign : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app) =>
		app.MapPost("/campaigns", async (
					CreateCampaignRequest request,
					ClaimsPrincipal claimsPrincipal,
					ISender sender,
					HttpContext httpContext,
					LinkGenerator linkGenerator) =>
				await sender
				   .Send(new CreateCampaignCommand(claimsPrincipal.GetUserId(), request.Name, request.Description))
				   .CreatedAsync(campaign =>
						new(linkGenerator.GetUriByName(httpContext, nameof(GetCampaign), new { id = campaign.Id })!)))
		   .WithName(nameof(CreateCampaign))
		   .WithTags(Tags.Campaigns)
		   .WithSummary("Create Campaign")
		   .WithDescription("Create a new campaign.")
		   .Accepts<CreateCampaignRequest>("application/json")
		   .Produces<CampaignResponse>(StatusCodes.Status201Created, "application/json")
		   .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, "application/json");

	internal sealed record CreateCampaignRequest(string Name, string Description);
}