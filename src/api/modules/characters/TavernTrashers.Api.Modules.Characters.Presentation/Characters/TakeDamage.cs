using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using TavernTrashers.Api.Common.Presentation.Endpoints;
using TavernTrashers.Api.Modules.Characters.Application.Characters;

namespace TavernTrashers.Api.Modules.Characters.Presentation.Characters;

public class TakeDamage : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app) =>
		app.MapPost("/characters/{id:guid}/hit-points/damage", async (
					Guid id,
					TakeDamageRequest request,
					ISender sender) =>
				await sender
				   .Send(new TakeDamageCommand(id, request.Amount))
				   .OkAsync())
		   .RequireAuthorization()
		   .WithName(nameof(TakeDamage))
		   .WithTags(Tags.Characters)
		   .WithSummary("Take Damage")
		   .WithDescription("Apply damage to a character. Temporary hit points are exhausted first; only overflow reduces current hit points.")
		   .Accepts<TakeDamageRequest>("application/json")
		   .Produces<HitPointsResponse>(StatusCodes.Status200OK)
		   .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
		   .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
		   .Produces<ProblemDetails>(StatusCodes.Status404NotFound);

	internal sealed record TakeDamageRequest(int Amount);
}
