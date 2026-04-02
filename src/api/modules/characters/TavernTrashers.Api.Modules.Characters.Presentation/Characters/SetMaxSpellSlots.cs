using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using TavernTrashers.Api.Common.Presentation.Endpoints;
using TavernTrashers.Api.Modules.Characters.Application.Characters;

namespace TavernTrashers.Api.Modules.Characters.Presentation.Characters;

public class SetMaxSpellSlots : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app) =>
		app.MapPut("/characters/{id:guid}/spell-slot-pools/{poolId:guid}/levels/{level:int}/max", async (
					Guid id,
					Guid poolId,
					int level,
					SetMaxSpellSlotsRequest request,
					ISender sender) =>
				await sender
				   .Send(new SetMaxSpellSlotsCommand(id, poolId, level, request.Max))
				   .OkAsync())
		   .RequireAuthorization()
		   .WithName(nameof(SetMaxSpellSlots))
		   .WithTags(Tags.Characters)
		   .WithSummary("Set Max Spell Slots")
		   .WithDescription("Set the maximum number of spell slots at the specified level.")
		   .Accepts<SetMaxSpellSlotsRequest>("application/json")
		   .Produces<SpellSlotPoolResponse>(StatusCodes.Status200OK)
		   .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
		   .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
		   .Produces<ProblemDetails>(StatusCodes.Status404NotFound);

	internal sealed record SetMaxSpellSlotsRequest(int Max);
}
