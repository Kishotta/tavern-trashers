using System.Security.Claims;
using TavernTrashers.Api.Common.Presentation.Authentication;
using TavernTrashers.Api.Modules.Users.Application.Users;

namespace TavernTrashers.Api.Modules.Users.Presentation.Users;

internal sealed class GetUserProfile : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app) =>
		app.MapGet("my/profile", async (ClaimsPrincipal claims, ISender sender) =>
				await sender
				   .Send(new GetUserQuery(claims.GetUserId()))
				   .OkAsync())
		   .RequireAuthorization()
		   .WithName(nameof(GetUserProfile))
		   .WithTags(Tags.Users);
}