using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using TavernTrashers.Api.Common.Presentation.Endpoints;
using TavernTrashers.Api.Modules.Characters.Application.Characters;

namespace TavernTrashers.Api.Modules.Characters.Presentation.Characters;

public class RestoreSlotPool : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app) =>
		app.MapPut("/characters/{id:guid}/spell-slot-pools/{poolId:guid}/restore", async (
					Guid id,
					Guid poolId,
					ISender sender) =>
				await sender
				   .Send(new RestoreSlotPoolCommand(id, poolId))
				   .OkAsync())
		   .RequireAuthorization()
		   .WithName(nameof(RestoreSlotPool))
		   .WithTags(Tags.Characters)
		   .WithSummary("Restore Slot Pool")
		   .WithDescription("Restore all spell slots in the specified pool to their maximum values.")
		   .Produces(StatusCodes.Status204NoContent)
		   .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
		   .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
		   .Produces<ProblemDetails>(StatusCodes.Status404NotFound);
}
