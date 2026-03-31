using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using TavernTrashers.Api.Common.Presentation.Endpoints;
using TavernTrashers.Api.Modules.Characters.Application.Characters;

namespace TavernTrashers.Api.Modules.Characters.Presentation.Characters;

public class ApplyMaxHpReduction : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app) =>
		app.MapPost("/characters/{id:guid}/hp/max-hp-reduction", async (
					Guid id,
					ApplyMaxHpReductionRequest request,
					ISender sender) =>
				await sender
				   .Send(new ApplyMaxHpReductionCommand(id, request.Reduction))
				   .OkAsync())
		   .RequireAuthorization()
		   .WithName(nameof(ApplyMaxHpReduction))
		   .WithTags(Tags.Characters)
		   .WithSummary("Apply Max HP Reduction")
		   .WithDescription("Apply a max HP reduction to a character. Effective Max HP decreases accordingly.")
		   .Accepts<ApplyMaxHpReductionRequest>("application/json")
		   .Produces<HpTrackerResponse>(StatusCodes.Status200OK)
		   .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
		   .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
		   .Produces<ProblemDetails>(StatusCodes.Status404NotFound);

	internal sealed record ApplyMaxHpReductionRequest(int Reduction);
}
