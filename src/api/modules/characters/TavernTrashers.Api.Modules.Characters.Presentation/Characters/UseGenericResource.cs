using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using TavernTrashers.Api.Common.Presentation.Endpoints;
using TavernTrashers.Api.Modules.Characters.Application.Characters;

namespace TavernTrashers.Api.Modules.Characters.Presentation.Characters;

public class UseGenericResource : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app) =>
		app.MapPut("/characters/{id:guid}/resources/{resourceId:guid}/use", async (
					Guid id,
					Guid resourceId,
					ISender sender) =>
				await sender
				   .Send(new UseGenericResourceCommand(id, resourceId))
				   .OkAsync())
		   .RequireAuthorization()
		   .WithName(nameof(UseGenericResource))
		   .WithTags(Tags.Characters)
		   .WithSummary("Use Generic Resource")
		   .WithDescription("Decrement a character resource by one.")
		   .Produces(StatusCodes.Status204NoContent)
		   .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
		   .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
		   .Produces<ProblemDetails>(StatusCodes.Status404NotFound);
}
