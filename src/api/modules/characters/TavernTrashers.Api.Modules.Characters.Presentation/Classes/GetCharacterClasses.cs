using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using TavernTrashers.Api.Common.Presentation.Endpoints;
using TavernTrashers.Api.Modules.Characters.Application.Classes;

namespace TavernTrashers.Api.Modules.Characters.Presentation.Classes;

public class GetCharacterClasses : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app) =>
		app.MapGet("/classes", async (
					ISender sender) =>
				await sender
				   .Send(new GetCharacterClassesQuery())
				   .OkAsync())
		   .WithName(nameof(GetCharacterClasses))
		   .WithTags(Tags.CharacterClasses)
		   .WithSummary("Get Character Classes")
		   .WithDescription("Retrieve all Character Classes.")
		   .Produces<CharacterClassResponse>(StatusCodes.Status200OK, "application/json");
}