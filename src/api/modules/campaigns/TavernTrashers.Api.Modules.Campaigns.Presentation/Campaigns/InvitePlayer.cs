using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using TavernTrashers.Api.Common.Presentation.Endpoints;
using TavernTrashers.Api.Modules.Campaigns.Application.Campaigns;

namespace TavernTrashers.Api.Modules.Campaigns.Presentation.Campaigns;

public class InvitePlayer : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app) =>
		app.MapPost("/campaigns/{campaignId:guid}/invitations",
				async (Guid campaignId, InvitePlayerRequest request, ISender sender) =>
					await sender.Send(new InvitePlayerCommand(campaignId, request.Email)).OkAsync())
		   .RequireAuthorization()
		   .WithName(nameof(InvitePlayer))
		   .WithTags(Tags.Campaigns)
		   .WithSummary("Invite Player")
		   .WithDescription("Invite a player to a given campaign.");

	internal sealed record InvitePlayerRequest(string Email);
}