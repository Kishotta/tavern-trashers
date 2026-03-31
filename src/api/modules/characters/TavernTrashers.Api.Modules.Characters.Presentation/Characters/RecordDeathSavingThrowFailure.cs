using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using TavernTrashers.Api.Common.Presentation.Endpoints;
using TavernTrashers.Api.Modules.Characters.Application.Characters;

namespace TavernTrashers.Api.Modules.Characters.Presentation.Characters;

public class RecordDeathSavingThrowFailure : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app) =>
		app.MapPost("/characters/{id:guid}/death-saving-throws/failure", async (
					Guid id,
					ISender sender) =>
				await sender
				   .Send(new RecordDeathSavingThrowFailureCommand(id))
				   .OkAsync())
		   .RequireAuthorization()
		   .WithName(nameof(RecordDeathSavingThrowFailure))
		   .WithTags(Tags.Characters)
		   .WithSummary("Record Death Saving Throw Failure")
		   .WithDescription("Record a death saving throw failure for a character. Maximum of 3 failures.")
		   .Produces<DeathSavingThrowsResponse>(StatusCodes.Status200OK)
		   .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
		   .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
		   .Produces<ProblemDetails>(StatusCodes.Status404NotFound);
}
