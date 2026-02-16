using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using TavernTrashers.Api.Common.Presentation.Endpoints;
using TavernTrashers.Api.Modules.Characters.Application.Classes;
using TavernTrashers.Api.Modules.Characters.Domain.Classes;

namespace TavernTrashers.Api.Modules.Characters.Presentation.Classes;

public class CreateCharacterClass : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app) =>
		app.MapPost("/classes", async (
					CreateCharacterClassRequest request,
					ISender sender) =>
				await sender
				   .Send(new CreateCharacterClassCommand(request.Name, request.ResourceDefinitions))
				   .OkAsync())
		   .WithName(nameof(CreateCharacterClass))
		   .WithTags(Tags.CharacterClasses)
		   .WithSummary("Create Character Class")
		   .WithDescription("Create a Character Class")
		   .Accepts<CreateCharacterClassRequest>("application/json")
		   .Produces<CharacterClass>()
		   .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
		   .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized);

	internal sealed record CreateCharacterClassRequest(string Name, List<ResourceDefinitionRequest> ResourceDefinitions);
}