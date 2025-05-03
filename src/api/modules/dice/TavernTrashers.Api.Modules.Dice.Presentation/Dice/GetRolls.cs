using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using TavernTrashers.Api.Common.Presentation.Endpoints;
using TavernTrashers.Api.Modules.Dice.Application.Dice;

namespace TavernTrashers.Api.Modules.Dice.Presentation.Dice;

public class GetRolls : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app) =>
		app.MapGet("/dice/rolls", async (
					ISender sender) =>
				await sender
				   .Send(new GetRollsQuery())
				   .OkAsync())
		   .WithName(nameof(GetRolls))
		   .WithTags(Tags.Dice)
		   .WithSummary("Get Rolls")
		   .WithDescription("Retrieve all dice rolls.")
		   .Produces<RollResponse>(StatusCodes.Status200OK, "application/json");
}