using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using TavernTrashers.Api.Common.Presentation.Endpoints;
using TavernTrashers.Api.Modules.Characters.Application.Characters;

namespace TavernTrashers.Api.Modules.Characters.Presentation.Characters;

public class SetHpFields : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app) =>
		app.MapPatch("/characters/{id:guid}/hp", async (
					Guid id,
					SetHpFieldsRequest request,
					ISender sender) =>
				await sender
				   .Send(new SetHpFieldsCommand(
						id,
						request.BaseMaxHp,
						request.CurrentHp,
						request.TemporaryHp,
						request.MaxHpReduction))
				   .OkAsync())
		   .RequireAuthorization()
		   .WithName(nameof(SetHpFields))
		   .WithTags(Tags.Characters)
		   .WithSummary("Set HP Fields (DM)")
		   .WithDescription("Directly set any HP field on a character. All fields are optional; only provided fields are updated.")
		   .Accepts<SetHpFieldsRequest>("application/json")
		   .Produces<HpTrackerResponse>(StatusCodes.Status200OK)
		   .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
		   .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
		   .Produces<ProblemDetails>(StatusCodes.Status404NotFound);

	internal sealed record SetHpFieldsRequest(
		int? BaseMaxHp,
		int? CurrentHp,
		int? TemporaryHp,
		int? MaxHpReduction);
}
