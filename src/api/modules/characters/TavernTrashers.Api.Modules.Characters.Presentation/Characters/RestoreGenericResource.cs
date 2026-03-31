using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using TavernTrashers.Api.Common.Presentation.Endpoints;
using TavernTrashers.Api.Modules.Characters.Application.Characters;

namespace TavernTrashers.Api.Modules.Characters.Presentation.Characters;

public class RestoreGenericResource : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app) =>
		app.MapPut("/characters/{id:guid}/resources/{resourceId:guid}/restore", async (
					Guid id,
					Guid resourceId,
					ISender sender) =>
				await sender
				   .Send(new RestoreGenericResourceCommand(id, resourceId))
				   .OkAsync())
		   .RequireAuthorization()
		   .WithName(nameof(RestoreGenericResource))
		   .WithTags(Tags.Characters)
		   .WithSummary("Restore Generic Resource")
		   .WithDescription("Restore a character resource to its default value.")
		   .Produces(StatusCodes.Status204NoContent)
		   .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
		   .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
		   .Produces<ProblemDetails>(StatusCodes.Status404NotFound);
}
