using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using TavernTrashers.Api.Common.Presentation.Endpoints;
using TavernTrashers.Api.Modules.Characters.Application.Characters;

namespace TavernTrashers.Api.Modules.Characters.Presentation.Characters;

public class SetTemporaryHp : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app) =>
		app.MapPut("/characters/{id:guid}/hp/temporary", async (
					Guid id,
					SetTemporaryHpRequest request,
					ISender sender) =>
				await sender
				   .Send(new SetTemporaryHpCommand(id, request.Amount))
				   .OkAsync())
		   .RequireAuthorization()
		   .WithName(nameof(SetTemporaryHp))
		   .WithTags(Tags.Characters)
		   .WithSummary("Set Temporary HP")
		   .WithDescription("Set temporary HP for a character. If temporary HP already exists, the higher value is kept.")
		   .Accepts<SetTemporaryHpRequest>("application/json")
		   .Produces<HpTrackerResponse>(StatusCodes.Status200OK)
		   .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
		   .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
		   .Produces<ProblemDetails>(StatusCodes.Status404NotFound);

	internal sealed record SetTemporaryHpRequest(int Amount);
}
