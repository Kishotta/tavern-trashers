using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using TavernTrashers.Api.Common.Presentation.Endpoints;
using TavernTrashers.Api.Modules.Characters.Application.Characters;

namespace TavernTrashers.Api.Modules.Characters.Presentation.Characters;

public class UseSpellSlot : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app) =>
		app.MapPut("/characters/{id:guid}/spell-slot-pools/{poolId:guid}/levels/{level:int}/use", async (
					Guid id,
					Guid poolId,
					int level,
					ISender sender) =>
				await sender
				   .Send(new UseSpellSlotCommand(id, poolId, level))
				   .OkAsync())
		   .RequireAuthorization()
		   .WithName(nameof(UseSpellSlot))
		   .WithTags(Tags.Characters)
		   .WithSummary("Use Spell Slot")
		   .WithDescription("Decrement the spell slot count at the specified level.")
		   .Produces(StatusCodes.Status204NoContent)
		   .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
		   .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
		   .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
		   .Produces<ProblemDetails>(StatusCodes.Status409Conflict);
}
