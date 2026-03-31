using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using TavernTrashers.Api.Common.Presentation.Endpoints;
using TavernTrashers.Api.Modules.Characters.Application.Characters;

namespace TavernTrashers.Api.Modules.Characters.Presentation.Characters;

public class ApplyMaxHitPointReduction : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app) =>
		app.MapPost("/characters/{id:guid}/hit-points/max-hit-point-reduction", async (
					Guid id,
					ApplyMaxHitPointReductionRequest request,
					ISender sender) =>
				await sender
				   .Send(new ApplyMaxHitPointReductionCommand(id, request.Reduction))
				   .OkAsync())
		   .RequireAuthorization()
		   .WithName(nameof(ApplyMaxHitPointReduction))
		   .WithTags(Tags.Characters)
		   .WithSummary("Apply Max Hit Point Reduction")
		   .WithDescription("Apply a max hit point reduction to a character. Effective max hit points decrease accordingly.")
		   .Accepts<ApplyMaxHitPointReductionRequest>("application/json")
		   .Produces<HitPointsResponse>(StatusCodes.Status200OK)
		   .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
		   .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
		   .Produces<ProblemDetails>(StatusCodes.Status404NotFound);

	internal sealed record ApplyMaxHitPointReductionRequest(int Reduction);
}
