using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using TavernTrashers.Api.Common.Presentation;
using TavernTrashers.Api.Modules.Campaigns.Application.Campaigns;
using TavernTrashers.Api.Modules.Campaigns.Application.Campaigns.CreateCampaign;

namespace TavernTrashers.Api.Modules.Campaigns.Presentation.Campaigns;

public class CreateCampaign : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPost("/campaigns", async (Request request, ISender sender, HttpContext httpContext, LinkGenerator linkGenerator) =>
			await sender
			   .Send(new CreateCampaignCommand(request.Name, request.Description))
			   .CreatedAsync(campaign => new Uri(linkGenerator.GetUriByName(httpContext, nameof(GetCampaign), new { id = campaign.Id })!)))
		   .WithName(nameof(CreateCampaign))
		   .WithTags(Tags.Campaigns)
		   .WithSummary("Create Campaign")
		   .WithDescription("Create a new campaign.")
		   .Accepts<Request>("application/json")
		   .Produces<CampaignResponse>(StatusCodes.Status201Created, "application/json")
		   .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, "application/json");
	}

	internal sealed record Request(string Name, string Description);
}