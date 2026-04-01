using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using TavernTrashers.Api.Common.Presentation.Endpoints;
using TavernTrashers.Api.Modules.Characters.Application.Characters;
using TavernTrashers.Api.Modules.Characters.Domain.Characters;

namespace TavernTrashers.Api.Modules.Characters.Presentation.Characters;

public class RemoveCondition : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app) =>
		app.MapDelete("/characters/{id:guid}/conditions", async (
					Guid id,
					RemoveConditionRequest request,
					ISender sender) =>
				await sender
				   .Send(new RemoveConditionCommand(id, request.Condition))
				   .OkAsync())
		   .RequireAuthorization()
		   .WithName(nameof(RemoveCondition))
		   .WithTags(Tags.Characters)
		   .WithSummary("Remove Condition")
		   .WithDescription("Remove a condition from a character. Implied conditions are cleared if no other active condition still implies them.")
		   .Accepts<RemoveConditionRequest>("application/json")
		   .Produces<CharacterResponse>(StatusCodes.Status200OK)
		   .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
		   .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
		   .Produces<ProblemDetails>(StatusCodes.Status404NotFound);

	internal sealed record RemoveConditionRequest(Conditions Condition);
}
