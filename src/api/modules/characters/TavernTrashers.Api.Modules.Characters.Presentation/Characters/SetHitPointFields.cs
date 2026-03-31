using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using TavernTrashers.Api.Common.Presentation.Endpoints;
using TavernTrashers.Api.Modules.Characters.Application.Characters;

namespace TavernTrashers.Api.Modules.Characters.Presentation.Characters;

public class SetHitPointFields : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app) =>
		app.MapPatch("/characters/{id:guid}/hit-points", async (
					Guid id,
					SetHitPointFieldsRequest request,
					ISender sender) =>
				await sender
				   .Send(new SetHitPointFieldsCommand(
						id,
						request.BaseMaxHitPoints,
						request.CurrentHitPoints,
						request.TemporaryHitPoints,
						request.MaxHitPointReduction))
				   .OkAsync())
		   .RequireAuthorization()
		   .WithName(nameof(SetHitPointFields))
		   .WithTags(Tags.Characters)
		   .WithSummary("Set Hit Point Fields (DM)")
		   .WithDescription("Directly set any hit point field on a character. All fields are optional; only provided fields are updated.")
		   .Accepts<SetHitPointFieldsRequest>("application/json")
		   .Produces<HitPointsResponse>(StatusCodes.Status200OK)
		   .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
		   .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
		   .Produces<ProblemDetails>(StatusCodes.Status404NotFound);

	internal sealed record SetHitPointFieldsRequest(
		int? BaseMaxHitPoints,
		int? CurrentHitPoints,
		int? TemporaryHitPoints,
		int? MaxHitPointReduction);
}
