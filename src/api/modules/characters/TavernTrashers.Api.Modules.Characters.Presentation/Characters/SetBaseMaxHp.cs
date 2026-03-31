using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using TavernTrashers.Api.Common.Presentation.Endpoints;
using TavernTrashers.Api.Modules.Characters.Application.Characters;

namespace TavernTrashers.Api.Modules.Characters.Presentation.Characters;

public class SetBaseMaxHp : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app) =>
		app.MapPut("/characters/{id:guid}/hp/base-max", async (
					Guid id,
					SetBaseMaxHpRequest request,
					ISender sender) =>
				await sender
				   .Send(new SetBaseMaxHpCommand(id, request.BaseMaxHp))
				   .OkAsync())
		   .RequireAuthorization()
		   .WithName(nameof(SetBaseMaxHp))
		   .WithTags(Tags.Characters)
		   .WithSummary("Set Base Max HP")
		   .WithDescription("Set the base maximum HP for a character. Also initializes the HP tracker if not yet set.")
		   .Accepts<SetBaseMaxHpRequest>("application/json")
		   .Produces<HpTrackerResponse>(StatusCodes.Status200OK)
		   .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
		   .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
		   .Produces<ProblemDetails>(StatusCodes.Status404NotFound);

	internal sealed record SetBaseMaxHpRequest(int BaseMaxHp);
}
