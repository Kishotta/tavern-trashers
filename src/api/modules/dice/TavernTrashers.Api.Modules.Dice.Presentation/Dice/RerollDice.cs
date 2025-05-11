using System.ComponentModel;
using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ModelContextProtocol.Server;
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

[McpServerToolType]
public static class RerollDiceTool
{
	private const string Description =
		"""
		Reroll specific dies in a roll.
		
		The original roll expression will be re-evaluated, and the die at the specified indices will be re-rolled.
		
		If no indices are provided, no dice will be re-rolled.

		""";

	[McpServerTool]
	[Description(Description)]
	public static async Task<string> RerollDice(
		ISender sender,
		[Description("The ID of the roll to reroll.")]
		Guid rollId,
		[Description("The indices of the dice to reroll. If empty, no dice will be rerolled.")]
		IReadOnlyList<int> diceIndices)
	{
		var result = await sender.Send(new RerollDiceCommand(rollId, diceIndices));
		return result.IsSuccess
			? JsonSerializer.Serialize(result.Value)
			: JsonSerializer.Serialize(result.Error);
	}
}