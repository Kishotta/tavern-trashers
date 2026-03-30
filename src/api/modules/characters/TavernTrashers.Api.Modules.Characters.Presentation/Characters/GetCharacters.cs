using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using TavernTrashers.Api.Common.Presentation.Endpoints;
using TavernTrashers.Api.Modules.Characters.Application.Characters;

namespace TavernTrashers.Api.Modules.Characters.Presentation.Characters;

public class GetCharacters : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app) =>
		app.MapGet("/characters", async (Guid campaignId, ISender sender) =>
				await sender
				   .Send(new GetCharactersQuery(campaignId))
				   .OkAsync())
		   .RequireAuthorization()
		   .WithName(nameof(GetCharacters))
		   .WithTags(Tags.Characters)
		   .WithSummary("Get Characters")
		   .WithDescription("Get all characters in a campaign.");
}
