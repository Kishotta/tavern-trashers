using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using TavernTrashers.Api.Common.Presentation.Endpoints;
using TavernTrashers.Api.Modules.Characters.Application.Characters;
using TavernTrashers.Api.Modules.Characters.Domain.Resources;

namespace TavernTrashers.Api.Modules.Characters.Presentation.Characters;

public class BulkRestoreGenericResources : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app) =>
		app.MapPost("/campaigns/{campaignId:guid}/generic-resources/bulk-restore", async (
					Guid campaignId,
					BulkRestoreGenericResourcesRequest request,
					ISender sender) =>
				await sender
				   .Send(new BulkRestoreGenericResourcesCommand(campaignId, request.Trigger))
				   .OkAsync())
		   .RequireAuthorization()
		   .WithName(nameof(BulkRestoreGenericResources))
		   .WithTags(Tags.Characters)
		   .WithSummary("Bulk Restore Generic Resources")
		   .WithDescription("Restore all generic resources matching a given reset trigger across all characters in a campaign.")
		   .Accepts<BulkRestoreGenericResourcesRequest>("application/json")
		   .Produces(StatusCodes.Status204NoContent)
		   .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
		   .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized);

	internal sealed record BulkRestoreGenericResourcesRequest(ResetTrigger Trigger);
}
