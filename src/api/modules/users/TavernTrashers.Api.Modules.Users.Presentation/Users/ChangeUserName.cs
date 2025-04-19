using TavernTrashers.Api.Modules.Users.Application.Users;

namespace TavernTrashers.Api.Modules.Users.Presentation.Users;

internal sealed class ChangeUserName : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app) =>
		app.MapPut("users/{id:guid}/profile", async (Guid id, ChangeUserNameRequest request, ISender sender) =>
				await sender.Send(new ChangeUserNameCommand(
						id,
						request.FirstName,
						request.LastName))
				   .OkAsync())
		   .RequireAuthorization(Permissions.ChangeUserName)
		   .WithName(nameof(ChangeUserName))
		   .WithTags(Tags.Users);

	internal sealed record ChangeUserNameRequest(string FirstName, string LastName);
}