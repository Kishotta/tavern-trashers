using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using TavernTrashers.Api.Common.Presentation.Endpoints;
using TavernTrashers.Api.Modules.Characters.Application.Characters;

namespace TavernTrashers.Api.Modules.Characters.Presentation.Characters;

public class RemoveGenericResource : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app) =>
		app.MapDelete("/characters/{characterId:guid}/generic-resources/{resourceId:guid}", async (
					Guid characterId,
					Guid resourceId,
					ISender sender) =>
				await sender
				   .Send(new RemoveGenericResourceCommand(characterId, resourceId))
				   .OkAsync())
		   .RequireAuthorization()
		   .WithName(nameof(RemoveGenericResource))
		   .WithTags(Tags.Characters)
		   .WithSummary("Remove Generic Resource")
		   .WithDescription("Remove a generic resource from a character.")
		   .Produces(StatusCodes.Status204NoContent)
		   .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
		   .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
		   .Produces<ProblemDetails>(StatusCodes.Status404NotFound);
}
