using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using TavernTrashers.Api.Common.Presentation.Endpoints;
using TavernTrashers.Api.Modules.Campaigns.Application.Members;

namespace TavernTrashers.Api.Modules.Campaigns.Presentation.Members;

public class GetMyInvitations : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app) =>
		app.MapGet("/my/invitations",
				async (ISender sender) =>
					await sender.Send(new GetInvitationsQuery()).OkAsync())
		   .RequireAuthorization()
		   .WithName(nameof(GetMyInvitations))
		   .WithTags(Tags.Users)
		   .WithSummary("Get Invitations")
		   .WithDescription("Get all invitations for the authenticated user.");
}