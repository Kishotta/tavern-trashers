using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using TavernTrashers.Api.Common.Presentation.Endpoints;
using TavernTrashers.Api.Modules.Characters.Application.Characters;
using TavernTrashers.Api.Modules.Characters.Domain.Resources;

namespace TavernTrashers.Api.Modules.Characters.Presentation.Characters;

public class AddGenericResource : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app) =>
		app.MapPost("/characters/{characterId:guid}/generic-resources", async (
					Guid characterId,
					AddGenericResourceRequest request,
					ISender sender) =>
				await sender
				   .Send(new AddGenericResourceCommand(
						characterId,
						request.Name,
						request.MaxAmount,
						request.Direction,
						request.ResetTriggers,
						request.SourceCategory))
				   .OkAsync())
		   .RequireAuthorization()
		   .WithName(nameof(AddGenericResource))
		   .WithTags(Tags.Characters)
		   .WithSummary("Add Generic Resource")
		   .WithDescription("Add a freeform resource to a character.")
		   .Accepts<AddGenericResourceRequest>("application/json")
		   .Produces<GenericResourceResponse>(StatusCodes.Status200OK)
		   .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
		   .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
		   .Produces<ProblemDetails>(StatusCodes.Status404NotFound);

	internal sealed record AddGenericResourceRequest(
		string Name,
		int MaxAmount,
		ResourceDirection Direction,
		ResetTrigger ResetTriggers,
		string? SourceCategory = null);
}
