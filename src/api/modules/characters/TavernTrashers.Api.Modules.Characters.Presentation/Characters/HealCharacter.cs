using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using TavernTrashers.Api.Common.Presentation.Endpoints;
using TavernTrashers.Api.Modules.Characters.Application.Characters;

namespace TavernTrashers.Api.Modules.Characters.Presentation.Characters;

public class HealCharacter : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app) =>
		app.MapPost("/characters/{id:guid}/hit-points/heal", async (
					Guid id,
					HealCharacterRequest request,
					ISender sender) =>
				await sender
				   .Send(new HealCharacterCommand(id, request.Amount))
				   .OkAsync())
		   .RequireAuthorization()
		   .WithName(nameof(HealCharacter))
		   .WithTags(Tags.Characters)
		   .WithSummary("Heal Character")
		   .WithDescription("Heal a character. Current hit points increase up to effective max hit points.")
		   .Accepts<HealCharacterRequest>("application/json")
		   .Produces<HitPointsResponse>(StatusCodes.Status200OK)
		   .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
		   .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
		   .Produces<ProblemDetails>(StatusCodes.Status404NotFound);

	internal sealed record HealCharacterRequest(int Amount);
}
