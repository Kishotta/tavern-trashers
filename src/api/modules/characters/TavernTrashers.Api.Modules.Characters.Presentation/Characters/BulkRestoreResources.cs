using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using TavernTrashers.Api.Common.Presentation.Endpoints;
using TavernTrashers.Api.Modules.Characters.Application.Characters;
using TavernTrashers.Api.Modules.Characters.Domain.Resources;

namespace TavernTrashers.Api.Modules.Characters.Presentation.Characters;

public class BulkRestoreResources : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app) =>
		app.MapPost("/campaigns/{campaignId:guid}/resources/restore", async (
					Guid campaignId,
					BulkRestoreResourcesRequest request,
					ISender sender) =>
				await sender
				   .Send(new BulkRestoreResourcesCommand(campaignId, request.Trigger))
				   .OkAsync())
		   .RequireAuthorization()
		   .WithName(nameof(BulkRestoreResources))
		   .WithTags(Tags.Characters)
		   .WithSummary("Bulk Restore Resources")
		   .WithDescription("Restore all resources matching a reset trigger across all characters in a campaign.")
		   .Accepts<BulkRestoreResourcesRequest>("application/json")
		   .Produces(StatusCodes.Status204NoContent)
		   .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
		   .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized);

	internal sealed record BulkRestoreResourcesRequest(ResetTrigger Trigger);
}
