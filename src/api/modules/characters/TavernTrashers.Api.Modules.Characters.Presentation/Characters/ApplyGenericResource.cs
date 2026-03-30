using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using TavernTrashers.Api.Common.Presentation.Endpoints;
using TavernTrashers.Api.Modules.Characters.Application.Characters;

namespace TavernTrashers.Api.Modules.Characters.Presentation.Characters;

public class ApplyGenericResource : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app) =>
		app.MapPost("/characters/{characterId:guid}/generic-resources/{resourceId:guid}/apply", async (
					Guid characterId,
					Guid resourceId,
					ISender sender) =>
				await sender
				   .Send(new ApplyGenericResourceCommand(characterId, resourceId))
				   .OkAsync())
		   .RequireAuthorization()
		   .WithName(nameof(ApplyGenericResource))
		   .WithTags(Tags.Characters)
		   .WithSummary("Apply Generic Resource")
		   .WithDescription("Increment an accumulating resource by one.")
		   .Produces(StatusCodes.Status204NoContent)
		   .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
		   .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
		   .Produces<ProblemDetails>(StatusCodes.Status404NotFound);
}
