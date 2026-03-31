using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using TavernTrashers.Api.Common.Presentation.Endpoints;
using TavernTrashers.Api.Modules.Characters.Application.Characters;

namespace TavernTrashers.Api.Modules.Characters.Presentation.Characters;

public class SetTemporaryHitPoints : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app) =>
		app.MapPut("/characters/{id:guid}/hit-points/temporary", async (
					Guid id,
					SetTemporaryHitPointsRequest request,
					ISender sender) =>
				await sender
				   .Send(new SetTemporaryHitPointsCommand(id, request.Amount))
				   .OkAsync())
		   .RequireAuthorization()
		   .WithName(nameof(SetTemporaryHitPoints))
		   .WithTags(Tags.Characters)
		   .WithSummary("Set Temporary Hit Points")
		   .WithDescription("Set temporary hit points for a character. If temporary hit points already exist, the higher value is kept.")
		   .Accepts<SetTemporaryHitPointsRequest>("application/json")
		   .Produces<HitPointsResponse>(StatusCodes.Status200OK)
		   .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
		   .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
		   .Produces<ProblemDetails>(StatusCodes.Status404NotFound);

	internal sealed record SetTemporaryHitPointsRequest(int Amount);
}
