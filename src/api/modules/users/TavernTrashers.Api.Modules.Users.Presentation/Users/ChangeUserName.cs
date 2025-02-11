using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Common.Presentation;
using TavernTrashers.Api.Common.Presentation.Endpoints;
using TavernTrashers.Api.Modules.Users.Application.Users.ChangeUserName;

namespace TavernTrashers.Api.Modules.Users.Presentation.Users;

internal sealed class ChangeUserName : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("users/{id:guid}/profile", async (Guid id, ChangeUserNameRequest request, ISender sender) =>
            {
                var result = await sender.Send(new ChangeUserNameCommand(
                    id,
                    request.FirstName,
                    request.LastName));

                return result.Match(Results.Ok, ApiResults.Problem);
            })
            .RequireAuthorization(Permissions.ChangeUserName)
            .WithName(nameof(ChangeUserName))
            .WithTags(Tags.Users);
    }

    internal sealed record ChangeUserNameRequest(string FirstName, string LastName);
}