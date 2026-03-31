using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using TavernTrashers.Api.Common.Presentation.Endpoints;
using TavernTrashers.Api.Modules.Characters.Application.Characters;

namespace TavernTrashers.Api.Modules.Characters.Presentation.Characters;

public class ResetDeathSavingThrows : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app) =>
		app.MapPost("/characters/{id:guid}/death-saving-throws/reset", async (
					Guid id,
					ISender sender) =>
				await sender
				   .Send(new ResetDeathSavingThrowsCommand(id))
				   .OkAsync())
		   .RequireAuthorization()
		   .WithName(nameof(ResetDeathSavingThrows))
		   .WithTags(Tags.Characters)
		   .WithSummary("Reset Death Saving Throws")
		   .WithDescription("Reset both death saving throw counters (successes and failures) to zero for a character.")
		   .Produces<DeathSavingThrowsResponse>(StatusCodes.Status200OK)
		   .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
		   .Produces<ProblemDetails>(StatusCodes.Status404NotFound);
}
