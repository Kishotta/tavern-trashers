using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Common.Infrastructure.Authentication;
using TavernTrashers.Api.Common.Presentation;
using TavernTrashers.Api.Common.Presentation.Endpoints;
using TavernTrashers.Api.Modules.Users.Application.Users.GetUser;

namespace TavernTrashers.Api.Modules.Users.Presentation.Users;

internal sealed class GetUserProfile : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
	    app.MapGet("users/profile", async (ClaimsPrincipal claims, ISender sender) =>
			    await sender
				   .Send(new GetUserQuery(claims.GetUserId()))
				   .OkAsync())
		   .RequireAuthorization(Permissions.GetUserProfile)
		   .WithName(nameof(GetUserProfile))
		   .WithTags(Tags.Users);
    }
}