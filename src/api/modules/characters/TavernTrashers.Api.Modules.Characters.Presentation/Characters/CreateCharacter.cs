using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using TavernTrashers.Api.Common.Presentation.Authentication;
using TavernTrashers.Api.Common.Presentation.Endpoints;
using TavernTrashers.Api.Modules.Characters.Application.Characters;

namespace TavernTrashers.Api.Modules.Characters.Presentation.Characters;

public class CreateCharacter : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app) =>
		app.MapPost("/characters", async (
					CreateCharacterRequest request,
					ClaimsPrincipal claims,
					ISender sender) =>
				await sender
				   .Send(new CreateCharacterCommand(
						request.Name,
						request.Level,
						claims.GetUserId(),
						request.CampaignId))
				   .OkAsync())
		   .RequireAuthorization()
		   .WithName(nameof(CreateCharacter))
		   .WithTags(Tags.Characters)
		   .WithSummary("Create Character")
		   .WithDescription("Create a new character associated with a campaign.")
		   .Accepts<CreateCharacterRequest>("application/json")
		   .Produces<CharacterResponse>(StatusCodes.Status200OK)
		   .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
		   .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized);

	internal sealed record CreateCharacterRequest(string Name, int Level, Guid CampaignId);
}
