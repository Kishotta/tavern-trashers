using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using TavernTrashers.Api.Common.Presentation.Endpoints;
using TavernTrashers.Api.Modules.Characters.Application.Classes;

namespace TavernTrashers.Api.Modules.Characters.Presentation.Classes;

public class CreateClass : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app) =>
		app.MapPost("/classes", async (
					CreateClassRequest request,
					ISender sender) =>
				await sender
				   .Send(new CreateClassCommand(request.Name))
				   .OkAsync())
		   .WithName(nameof(CreateClass))
		   .WithTags(Tags.Classes)
		   .WithSummary("Create Class")
		   .WithDescription("Create a D&D Class")
		   .Accepts<CreateClassRequest>("application/json")
		   .Produces<ClassResponse>(StatusCodes.Status200OK, "application/json")
		   .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
		   .Produces<ProblemDetails>(StatusCodes.Status409Conflict);

	internal sealed record CreateClassRequest(string Name);
}
