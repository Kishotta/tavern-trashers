using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using TavernTrashers.Api.Common.Presentation;
using TavernTrashers.Api.Modules.Campaigns.Application.Campaigns.GetCampaigns;
using TavernTrashers.Api.Modules.Campaigns.Application.Questionnaires.CreateQuestionnaire;
using TavernTrashers.Api.Modules.Campaigns.Presentation.Campaigns;

namespace TavernTrashers.Api.Modules.Campaigns.Presentation.Questionnaires;

public sealed class CreateQuestionnaire : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPost("/questionnaires", async (Request request, ISender sender, HttpContext httpContext, LinkGenerator linkGenerator) =>
				await sender
				   .Send(new CreateQuestionnaireCommand(request.CampaignId, request.Title, request.Description))
				   .CreatedAsync(questionnaire => new Uri(linkGenerator.GetUriByName(httpContext, nameof(GetCampaign), new { id = questionnaire.Id })!)))
		   .WithName(nameof(GetCampaigns))
		   .WithTags(Tags.Campaigns)
		   .WithSummary("Get all Campaigns")
		   .WithDescription("Get all campaigns.");
	}

	internal sealed record Request(Guid CampaignId, string Title, string Description);
}