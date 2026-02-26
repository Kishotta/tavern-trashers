using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using TavernTrashers.Api.Common.Presentation.Endpoints;
using TavernTrashers.Api.Modules.Characters.Application.Classes;

namespace TavernTrashers.Api.Modules.Characters.Presentation.Classes;

public class RenameClass : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app) =>
		app.MapPut("/classes/{classId:guid}", async (
					Guid classId,
					RenameClassRequest request,
					ISender sender) =>
				await sender
				   .Send(new RenameClassCommand(classId, request.Name))
				   .OkAsync())
		   .WithName(nameof(RenameClass))
		   .WithTags(Tags.Classes)
		   .WithSummary("Rename Class")
		   .WithDescription("Rename a D&D Class")
		   .Accepts<RenameClassRequest>("application/json")
		   .Produces(StatusCodes.Status200OK)
		   .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
		   .Produces<ProblemDetails>(StatusCodes.Status404NotFound);

	internal sealed record RenameClassRequest(string Name);
}
