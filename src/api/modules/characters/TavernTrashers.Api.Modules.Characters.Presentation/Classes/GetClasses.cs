using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using TavernTrashers.Api.Common.Presentation.Endpoints;
using TavernTrashers.Api.Modules.Characters.Application.Classes;

namespace TavernTrashers.Api.Modules.Characters.Presentation.Classes;

public class GetClasses : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app) =>
		app.MapGet("/classes", async (
					ISender sender) =>
				await sender
				   .Send(new GetClassesQuery())
				   .OkAsync())
		   .WithName(nameof(GetClasses))
		   .WithTags(Tags.Classes)
		   .WithSummary("Get Classes")
		   .WithDescription("Retrieve all D&D Classes.")
		   .Produces<IReadOnlyCollection<ClassResponse>>(StatusCodes.Status200OK, "application/json");
}
