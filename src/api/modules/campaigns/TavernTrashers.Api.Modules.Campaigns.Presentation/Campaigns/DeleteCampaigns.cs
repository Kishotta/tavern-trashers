using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using TavernTrashers.Api.Common.Presentation;
using TavernTrashers.Api.Modules.Campaigns.Application.Campaigns.DeleteCampaign;

namespace TavernTrashers.Api.Modules.Campaigns.Presentation.Campaigns;

public class DeleteCampaign : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapDelete("/campaigns/{id:guid}", async (Guid id, ISender sender) =>
			await sender
			   .Send(new DeleteCampaignCommand(id))
			   .OkAsync())
			.WithName(nameof(DeleteCampaign))
		   .WithTags(Tags.Campaigns)
		   .WithSummary("Delete Campaign")
		   .WithDescription("Permanently delete a campaign.");
	}
}