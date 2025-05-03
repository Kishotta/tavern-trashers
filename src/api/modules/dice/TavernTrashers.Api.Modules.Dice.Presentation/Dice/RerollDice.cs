using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using TavernTrashers.Api.Common.Presentation.Endpoints;
using TavernTrashers.Api.Modules.Dice.Application.Dice;

namespace TavernTrashers.Api.Modules.Dice.Presentation.Dice;

public class RerollDice : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app) =>
		app.MapPost("/dice/rolls/reroll", async (
					RerollDiceRequest request,
					ISender sender) =>
				await sender
				   .Send(new RerollDiceCommand(request.RollId, request.DiceIndices))
				   .OkAsync())
		   .WithName(nameof(RerollDice))
		   .WithTags(Tags.Dice)
		   .WithSummary("Reroll Dice")
		   .WithDescription("Reroll some or all of a specific dice roll and return the new result.")
		   .Accepts<RerollDiceRequest>("application/json")
		   .Produces<RollResponse>(StatusCodes.Status200OK, "application/json")
		   .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, "application/json");

	internal sealed record RerollDiceRequest(Guid RollId, IReadOnlyList<int> DiceIndices);
}