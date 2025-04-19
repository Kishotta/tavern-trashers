using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using TavernTrashers.Api.Common.Presentation.Endpoints;
using TavernTrashers.Api.Modules.Campaigns.Application.Campaigns;

namespace TavernTrashers.Api.Modules.Campaigns.Presentation.Campaigns;

public class GetCampaigns : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app) =>
		app.MapGet("/campaigns", async (ISender sender) =>
				await sender
				   .Send(new GetCampaignsQuery())
				   .OkAsync())
		   .WithName(nameof(GetCampaigns))
		   .WithTags(Tags.Campaigns)
		   .WithSummary("Get all Campaigns")
		   .WithDescription("Get all campaigns.");
}