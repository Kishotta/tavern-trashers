using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using TavernTrashers.Api.Common.Presentation.Endpoints;
using TavernTrashers.Api.Modules.Dice.Application.Dice;

namespace TavernTrashers.Api.Modules.Dice.Presentation.Dice;

public class GetRoll : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app) =>
		app.MapGet("/dice/rolls/{rollId:guid}", async (
					Guid rollId,
					ISender sender) =>
				await sender
				   .Send(new GetRollQuery(rollId))
				   .OkAsync())
		   .WithName(nameof(GetRoll))
		   .WithTags(Tags.Dice)
		   .WithSummary("Get Roll")
		   .WithDescription("Retrieve a specific dice roll by its ID.")
		   .Produces<RollResponse>(StatusCodes.Status200OK, "application/json")
		   .Produces<ProblemDetails>(StatusCodes.Status404NotFound, "application/json");
}