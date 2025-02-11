using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Common.Presentation;
using TavernTrashers.Api.Common.Presentation.Endpoints;
using TavernTrashers.Api.Modules.Users.Application.Users.RegisterUser;

namespace TavernTrashers.Api.Modules.Users.Presentation.Users;

internal sealed class RegisterUser : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPost("users/register", async (
				RegisterUserRequest request, 
				ISender sender, 
				HttpContext context, 
				LinkGenerator linkGenerator) =>
			{
				var result = await sender.Send(new RegisterUserCommand(
					request.Email,
					request.Password,
					request.FirstName,
					request.LastName));

				return result.Match(
					() => Results.Created(
						linkGenerator.GetUriByName(context, nameof(GetUserProfile), new { id = result.Value.Id }),
						result.Value),
					ApiResults.Problem);
			})
		   .AllowAnonymous()
		   .WithName(nameof(RegisterUser))
		   .WithTags(Tags.Users);
	}

	internal sealed record RegisterUserRequest(string Email, string Password, string FirstName, string LastName);
}