using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using TavernTrashers.Api.Common.Presentation.Endpoints;
using TavernTrashers.Api.Modules.Dice.Application.Dice;

namespace TavernTrashers.Api.Modules.Dice.Presentation.Dice;

public class RollDice : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app) =>
		app.MapPost("/dice/rolls", async (
					RollDiceRequest request,
					ISender sender) =>
				await sender
				   .Send(new RollDiceCommand(request.Expression))
				   .OkAsync())
		   .WithName(nameof(RollDice))
		   .WithTags(Tags.Dice)
		   .WithSummary("Roll Dice")
		   .WithDescription("Evaluate a dice expression and return the result.")
		   .Accepts<RollDiceRequest>("application/json")
		   .Produces<RollResponse>(StatusCodes.Status200OK, "application/json")
		   .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, "application/json");

	internal sealed record RollDiceRequest(string Expression);
}