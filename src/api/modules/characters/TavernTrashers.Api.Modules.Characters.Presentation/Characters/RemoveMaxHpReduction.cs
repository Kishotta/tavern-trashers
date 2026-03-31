using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using TavernTrashers.Api.Common.Presentation.Endpoints;
using TavernTrashers.Api.Modules.Characters.Application.Characters;

namespace TavernTrashers.Api.Modules.Characters.Presentation.Characters;

public class RemoveMaxHpReduction : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app) =>
		app.MapDelete("/characters/{id:guid}/hp/max-hp-reduction", async (
					Guid id,
					ISender sender) =>
				await sender
				   .Send(new RemoveMaxHpReductionCommand(id))
				   .OkAsync())
		   .RequireAuthorization()
		   .WithName(nameof(RemoveMaxHpReduction))
		   .WithTags(Tags.Characters)
		   .WithSummary("Remove Max HP Reduction")
		   .WithDescription("Remove the max HP reduction from a character. Effective Max HP returns to Base Max HP.")
		   .Produces<HpTrackerResponse>(StatusCodes.Status200OK)
		   .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
		   .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
		   .Produces<ProblemDetails>(StatusCodes.Status404NotFound);
}
