using TavernTrashers.Api.Modules.Users.Application.Users;

namespace TavernTrashers.Api.Modules.Users.Presentation.Users;

internal sealed class RegisterUser : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app) =>
		app.MapPost("users/register", async (
					RegisterUserRequest request,
					ISender sender) =>
				await sender.Send(
						new RegisterUserCommand(
							request.Email,
							request.Password,
							request.FirstName,
							request.LastName))
				   .OkAsync())
		   .AllowAnonymous()
		   .WithName(nameof(RegisterUser))
		   .WithTags(Tags.Users);

	internal sealed record RegisterUserRequest(
		string Email,
		string Password,
		string FirstName,
		string LastName);
}