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
		   .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, "application/json")
		   .Produces<ProblemDetails>(StatusCodes.Status404NotFound, "application/json");

	internal sealed record RollDiceRequest(string Expression);
}

[McpServerToolType]
public static class RollDiceTool
{
	private const string Description =
		"""
		Evaluate a dice expression and return the result.

		The dice expression supports multiple features:

		- Constants: `1`, `2`, `3`, etc.
		- Dice notation: `d4`, `2d6`, `3d8`, etc.
		- Fate dice notation: `df`, `2df`, etc. rolling a +1, 0, or -1.
		- Keep/Drop notation: `2d20kh`, `4d6kh3`, `3d10dl2`, etc.
		- Exploding dice: `d6!`, `2d8!`, etc. that trigger an additional dice roll on the maximum value.
		- Arithmetic Operators: `+`, `-`, `*`, `/`, `%`.
		- Parentheses for grouping: `(`, `)`.

		Some common Dungeons and Dragons phrases can be represented with these expressions:

		- A "D20 Test", "Skill Check", "Ability Check", "Saving Throw", or "Attack Roll" can be represented as `d20 + {modifier}`.
		- Any roll made "with advantage" can be represented as `2d20kh1 + {modifier}`.
		- Any roll made "with disadvantage" can be represented as `2d20kl1 + {modifier}`.
		- An "Ability Score Determination" is made by rolling 4d6 and dropping the lowest die, which can be represented as `4d6dl1`.

		The results of a roll are returned in a json object with the following properties:

		- Id: A unique identifier for the roll.
		- Expression: The original expression that was evaluated.
		- Total: The total value of the roll.
		- Minimum: The theoretical minimum value of the roll.
		- Maximum: The theoretical maximum value of the roll.
		- Average: The average value of the roll.
		- RawRolls: The raw values of all the dice that were rolled.
		- KeptRolls: The raw values of the dice that were kept.
		- RolledAtUtc: The date and time the roll was made.

		""";

	[McpServerTool]
	[Description(Description)]
	public static async Task<string> RollDice(
		ISender sender,
		[Description("The dice expression to evaluate.")]
		string expression)
	{
		var result = await sender.Send(new RollDiceCommand(expression));
		return result.IsSuccess
			? JsonSerializer.Serialize(result.Value)
			: JsonSerializer.Serialize(result.Error);
	}
}