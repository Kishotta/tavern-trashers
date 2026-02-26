using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using TavernTrashers.Api.Common.Presentation.Endpoints;
using TavernTrashers.Api.Modules.Characters.Application.Classes;

namespace TavernTrashers.Api.Modules.Characters.Presentation.Classes;

public class GetClass : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app) =>
		app.MapGet("/classes/{classId:guid}", async (
					Guid classId,
					ISender sender) =>
				await sender
				   .Send(new GetClassQuery(classId))
				   .OkAsync())
		   .WithName(nameof(GetClass))
		   .WithTags(Tags.Classes)
		   .WithSummary("Get Class")
		   .WithDescription("Retrieve a D&D Class by ID.")
		   .Produces<ClassResponse>(StatusCodes.Status200OK, "application/json")
		   .Produces<ProblemDetails>(StatusCodes.Status404NotFound, "application/json");
}
