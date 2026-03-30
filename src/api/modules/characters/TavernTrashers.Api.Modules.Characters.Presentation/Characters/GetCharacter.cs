using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using TavernTrashers.Api.Common.Presentation.Endpoints;
using TavernTrashers.Api.Modules.Characters.Application.Characters;

namespace TavernTrashers.Api.Modules.Characters.Presentation.Characters;

public class GetCharacter : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app) =>
		app.MapGet("/characters/{id:guid}", async (Guid id, ISender sender) =>
				await sender
				   .Send(new GetCharacterQuery(id))
				   .OkAsync())
		   .RequireAuthorization()
		   .WithName(nameof(GetCharacter))
		   .WithTags(Tags.Characters)
		   .WithSummary("Get Character")
		   .WithDescription("Get a character by ID.")
		   .Produces<CharacterResponse>(StatusCodes.Status200OK)
		   .Produces<ProblemDetails>(StatusCodes.Status404NotFound);
}
