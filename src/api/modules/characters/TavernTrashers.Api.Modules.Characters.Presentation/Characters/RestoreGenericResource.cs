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
		app.MapPost("/characters/{characterId:guid}/generic-resources/{resourceId:guid}/restore", async (
					Guid characterId,
					Guid resourceId,
					ISender sender) =>
				await sender
				   .Send(new RestoreGenericResourceCommand(characterId, resourceId))
				   .OkAsync())
		   .RequireAuthorization()
		   .WithName(nameof(RestoreGenericResource))
		   .WithTags(Tags.Characters)
		   .WithSummary("Restore Generic Resource")
		   .WithDescription("Restore a resource to its default value (Spending → max, Accumulating → 0).")
		   .Produces(StatusCodes.Status204NoContent)
		   .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
		   .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
		   .Produces<ProblemDetails>(StatusCodes.Status404NotFound);
}
