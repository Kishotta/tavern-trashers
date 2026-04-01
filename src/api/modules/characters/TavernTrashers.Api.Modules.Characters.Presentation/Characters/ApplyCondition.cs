using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using TavernTrashers.Api.Common.Presentation.Endpoints;
using TavernTrashers.Api.Modules.Characters.Application.Characters;
using TavernTrashers.Api.Modules.Characters.Domain.Characters;

namespace TavernTrashers.Api.Modules.Characters.Presentation.Characters;

public class ApplyCondition : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app) =>
		app.MapPost("/characters/{id:guid}/conditions", async (
					Guid id,
					ApplyConditionRequest request,
					ISender sender) =>
				await sender
				   .Send(new ApplyConditionCommand(id, request.Condition))
				   .OkAsync())
		   .RequireAuthorization()
		   .WithName(nameof(ApplyCondition))
		   .WithTags(Tags.Characters)
		   .WithSummary("Apply Condition")
		   .WithDescription("Apply a condition to a character. Implied conditions are automatically applied.")
		   .Accepts<ApplyConditionRequest>("application/json")
		   .Produces<CharacterResponse>(StatusCodes.Status200OK)
		   .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
		   .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
		   .Produces<ProblemDetails>(StatusCodes.Status404NotFound);

	internal sealed record ApplyConditionRequest(Condition Condition);
}
