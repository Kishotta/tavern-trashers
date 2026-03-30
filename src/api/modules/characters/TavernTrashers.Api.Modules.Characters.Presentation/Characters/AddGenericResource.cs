using System.Security.Claims;
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
		app.MapPost("/characters/{id:guid}/resources", async (
					Guid id,
					AddGenericResourceRequest request,
					ISender sender) =>
				await sender
				   .Send(new AddGenericResourceCommand(
						id,
						request.Name,
						request.MaxUses,
						request.Direction,
						request.SourceCategory,
						request.ResetTriggers))
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
		int MaxUses,
		ResourceDirection Direction,
		SourceCategory SourceCategory,
		IReadOnlyCollection<ResetTrigger> ResetTriggers);
}
