using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using TavernTrashers.Api.Common.Presentation.Endpoints;
using TavernTrashers.Api.Modules.Characters.Application.Characters;

namespace TavernTrashers.Api.Modules.Characters.Presentation.Characters;

public class AddPactMagicSlotPool : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app) =>
		app.MapPost("/characters/{id:guid}/spell-slot-pools/pact-magic", async (
					Guid id,
					ISender sender) =>
				await sender
				   .Send(new AddPactMagicSlotPoolCommand(id))
				   .OkAsync())
		   .RequireAuthorization()
		   .WithName(nameof(AddPactMagicSlotPool))
		   .WithTags(Tags.Characters)
		   .WithSummary("Add Pact Magic Slot Pool")
		   .WithDescription("Add a Pact Magic spell slot pool to a character.")
		   .Produces<SpellSlotPoolResponse>(StatusCodes.Status200OK)
		   .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
		   .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
		   .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
		   .Produces<ProblemDetails>(StatusCodes.Status409Conflict);
}
