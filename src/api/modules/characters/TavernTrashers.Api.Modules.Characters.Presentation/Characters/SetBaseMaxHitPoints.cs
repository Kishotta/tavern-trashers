using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using TavernTrashers.Api.Common.Presentation.Endpoints;
using TavernTrashers.Api.Modules.Characters.Application.Characters;

namespace TavernTrashers.Api.Modules.Characters.Presentation.Characters;

public class SetBaseMaxHitPoints : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app) =>
		app.MapPut("/characters/{id:guid}/hit-points/base-max", async (
					Guid id,
					SetBaseMaxHitPointsRequest request,
					ISender sender) =>
				await sender
				   .Send(new SetBaseMaxHitPointsCommand(id, request.BaseMaxHitPoints))
				   .OkAsync())
		   .RequireAuthorization()
		   .WithName(nameof(SetBaseMaxHitPoints))
		   .WithTags(Tags.Characters)
		   .WithSummary("Set Base Max Hit Points")
		   .WithDescription("Set the base maximum hit points for a character. Also initializes the hit points tracker if not yet set.")
		   .Accepts<SetBaseMaxHitPointsRequest>("application/json")
		   .Produces<HitPointsResponse>(StatusCodes.Status200OK)
		   .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
		   .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
		   .Produces<ProblemDetails>(StatusCodes.Status404NotFound);

	internal sealed record SetBaseMaxHitPointsRequest(int BaseMaxHitPoints);
}
