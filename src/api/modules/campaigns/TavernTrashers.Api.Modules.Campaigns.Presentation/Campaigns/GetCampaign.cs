using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using TavernTrashers.Api.Common.Presentation;
using TavernTrashers.Api.Common.Presentation.Endpoints;
using TavernTrashers.Api.Modules.Campaigns.Application.Campaigns.GetCampaign;

namespace TavernTrashers.Api.Modules.Campaigns.Presentation.Campaigns;

public class GetCampaign : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet("/campaigns/{id:guid}", async (Guid id, ISender sender) =>
				await sender
				   .Send(new GetCampaignQuery(id))
				   .OkAsync())
		   .WithName(nameof(GetCampaign))
		   .WithTags(Tags.Campaigns)
		   .WithSummary("Get Campaign by Id")
		   .WithDescription("Get Campaign by Id.");
	}
}