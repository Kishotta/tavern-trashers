using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using TavernTrashers.Api.Common.Presentation.Endpoints;
using TavernTrashers.Api.Modules.Characters.Application.Classes;

namespace TavernTrashers.Api.Modules.Characters.Presentation.Classes;

public class DeleteClass : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app) =>
		app.MapDelete("/classes/{classId:guid}", async (
					Guid classId,
					ISender sender) =>
				await sender
				   .Send(new DeleteClassCommand(classId))
				   .OkAsync())
		   .WithName(nameof(DeleteClass))
		   .WithTags(Tags.Classes)
		   .WithSummary("Delete Class")
		   .WithDescription("Soft delete a D&D Class")
		   .Produces(StatusCodes.Status200OK)
		   .Produces<ProblemDetails>(StatusCodes.Status404NotFound);
}
