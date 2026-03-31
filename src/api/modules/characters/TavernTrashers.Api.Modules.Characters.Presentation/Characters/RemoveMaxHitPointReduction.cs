using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using TavernTrashers.Api.Common.Presentation.Endpoints;
using TavernTrashers.Api.Modules.Characters.Application.Characters;

namespace TavernTrashers.Api.Modules.Characters.Presentation.Characters;

public class RemoveMaxHitPointReduction : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app) =>
		app.MapDelete("/characters/{id:guid}/hit-points/max-hit-point-reduction", async (
					Guid id,
					ISender sender) =>
				await sender
				   .Send(new RemoveMaxHitPointReductionCommand(id))
				   .OkAsync())
		   .RequireAuthorization()
		   .WithName(nameof(RemoveMaxHitPointReduction))
		   .WithTags(Tags.Characters)
		   .WithSummary("Remove Max Hit Point Reduction")
		   .WithDescription("Remove the max hit point reduction from a character. Effective max hit points return to base max hit points.")
		   .Produces<HitPointsResponse>(StatusCodes.Status200OK)
		   .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
		   .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
		   .Produces<ProblemDetails>(StatusCodes.Status404NotFound);
}
