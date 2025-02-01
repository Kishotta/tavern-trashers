using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Modules.Campaigns.Application.Abstractions.Data;
using TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns;
using TavernTrashers.Api.Modules.Campaigns.Domain.Questionnaires;

namespace TavernTrashers.Api.Modules.Campaigns.Application.Questionnaires.CreateQuestionnaire;

public sealed record CreateQuestionnaireCommand(Guid CampaignId, string Title, string Description)
	: ICommand<QuestionnaireResponse>;

public sealed record QuestionnaireResponse(Guid Id, Guid CampaignId, string Title, string Description)
{
	public static implicit operator QuestionnaireResponse(Questionnaire questionnaire)
		=> new(questionnaire.Id, questionnaire.Campaign.Id, questionnaire.Title, questionnaire.Description);
}

internal sealed class CreateQuestionnaireCommandHandler(
	ICampaignRepository campaigns,
	IQuestionnaireRepository questionnaires,
	IUnitOfWork unitOfWork) 
	: ICommandHandler<CreateQuestionnaireCommand, QuestionnaireResponse>
{
	public async Task<Result<QuestionnaireResponse>> Handle(CreateQuestionnaireCommand request, CancellationToken cancellationToken)
	{
		var campaign = await campaigns.GetAsync(request.CampaignId, cancellationToken);
		if (campaign.IsFailure)
			return campaign.Error;
		
		var questionnaire = Questionnaire.Create(campaign, request.Title, request.Description);
		
		questionnaires.Add(questionnaire);
		
		await unitOfWork.SaveChangesAsync(cancellationToken);

		return (QuestionnaireResponse)questionnaire;
	}
}