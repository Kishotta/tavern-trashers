using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using TavernTrashers.Api.Common.Presentation.Endpoints;
using TavernTrashers.Api.Modules.Characters.Application.Classes;

namespace TavernTrashers.Api.Modules.Characters.Presentation.Classes;

public class GetCharacterClassResourcesAtLevel : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app) =>
		app.MapGet("/classes/{classId:guid}/resources", async (
					Guid classId,
					int level,
					ISender sender) =>
				await sender
				   .Send(new GetCharacterClassResourcesAtLevelQuery(classId, level))
				   .OkAsync())
		   .WithName(nameof(GetCharacterClassResourcesAtLevel))
		   .WithTags(Tags.CharacterClasses)
		   .WithSummary("Get Character Class Resources at Level")
		   .WithDescription("Retrieve the resources available for a specific class at a specific level.")
		   .Produces<GetCharacterClassResourcesAtLevelResponse>(StatusCodes.Status200OK, "application/json")
		   .Produces<ProblemDetails>(StatusCodes.Status404NotFound, "application/json");
}