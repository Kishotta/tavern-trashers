using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using TavernTrashers.Api.Common.Presentation.Endpoints;
using TavernTrashers.Api.Modules.Campaigns.Application.Campaigns;

namespace TavernTrashers.Api.Modules.Campaigns.Presentation.Campaigns;

public class GetCampaign : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app) =>
		app.MapGet("/campaigns/{campaignId:guid}",
				async (Guid campaignId, ISender sender) =>
					await sender
					   .Send(new GetCampaignQuery(campaignId))
					   .OkAsync())
		   .RequireAuthorization()
		   .WithName(nameof(GetCampaign))
		   .WithTags(Tags.Campaigns)
		   .WithSummary("Get Campaign by Id")
		   .WithDescription("Get Campaign by Id.");
}