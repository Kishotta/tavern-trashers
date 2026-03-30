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
		app.MapPost("/characters/{characterId:guid}/generic-resources/{resourceId:guid}/use", async (
					Guid characterId,
					Guid resourceId,
					ISender sender) =>
				await sender
				   .Send(new UseGenericResourceCommand(characterId, resourceId))
				   .OkAsync())
		   .RequireAuthorization()
		   .WithName(nameof(UseGenericResource))
		   .WithTags(Tags.Characters)
		   .WithSummary("Use Generic Resource")
		   .WithDescription("Decrement a spending resource by one.")
		   .Produces(StatusCodes.Status204NoContent)
		   .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
		   .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
		   .Produces<ProblemDetails>(StatusCodes.Status404NotFound);
}
