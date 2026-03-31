using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using TavernTrashers.Api.Common.Presentation.Endpoints;
using TavernTrashers.Api.Modules.Characters.Application.Characters;

namespace TavernTrashers.Api.Modules.Characters.Presentation.Characters;

public class RecordDeathSavingThrowSuccess : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app) =>
		app.MapPost("/characters/{id:guid}/death-saving-throws/success", async (
					Guid id,
					ISender sender) =>
				await sender
				   .Send(new RecordDeathSavingThrowSuccessCommand(id))
				   .OkAsync())
		   .RequireAuthorization()
		   .WithName(nameof(RecordDeathSavingThrowSuccess))
		   .WithTags(Tags.Characters)
		   .WithSummary("Record Death Saving Throw Success")
		   .WithDescription("Record a death saving throw success for a character. Maximum of 3 successes.")
		   .Produces<DeathSavingThrowsResponse>(StatusCodes.Status200OK)
		   .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
		   .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
		   .Produces<ProblemDetails>(StatusCodes.Status404NotFound);
}
