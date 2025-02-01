using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Modules.Campaigns.Application.Abstractions.Data;
using TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns;
using TavernTrashers.Api.Modules.Campaigns.Domain.Questionnaires;
using TavernTrashers.Api.Modules.Campaigns.Domain.Questions;

namespace TavernTrashers.Api.Modules.Campaigns.Application.Questionnaires;

public class QuestionnaireService(
	ICampaignRepository campaignRepository,
	IQuestionnaireRepository questionnaireRepository,
	IUnitOfWork unitOfWork)
{
	public async Task<Result<Questionnaire>> AddQuestionnaireAsync(
		Guid campaignId, 
		string title, 
		string description,
		CancellationToken cancellationToken = default)
	{
		var campaign = await campaignRepository.GetAsync(campaignId, cancellationToken);
		if (campaign.IsFailure) 
			return campaign.Error;
		
		var questionnaire = Questionnaire.Create(campaign, title, description);
		
		questionnaireRepository.Add(questionnaire);

		await unitOfWork.SaveChangesAsync(cancellationToken);

		return questionnaire;
	}
	
	public async Task<Result<Questionnaire>> AddQuestionToQuestionnaireAsync(
		Guid questionnaireId,
		string prompt,
		QuestionType type,
		CancellationToken cancellationToken = default)
	{
		var questionnaire = await questionnaireRepository.GetAsync(questionnaireId, cancellationToken);
		if (questionnaire.IsFailure) 
			return questionnaire.Error;
		
		var question = questionnaire.Value.AddQuestion(prompt, type);
		if (question.IsFailure)
			return question.Error;
		
		await unitOfWork.SaveChangesAsync(cancellationToken);

		return questionnaire;
	}
	
	public async Task<Result<Questionnaire>> RemoveQuestionFromQuestionnaireAsync(
		Guid questionnaireId,
		Guid questionId,
		CancellationToken cancellationToken = default)
	{
		var questionnaire = await questionnaireRepository.GetAsync(questionnaireId, cancellationToken);
		if (questionnaire.IsFailure) 
			return questionnaire.Error;

		var result = questionnaire.Value.RemoveQuestion(questionId);
		if (result.IsFailure)
			return result.Error;
		
		await unitOfWork.SaveChangesAsync(cancellationToken);

		return questionnaire;
	}
	
	public async Task<Result<Questionnaire>> AddChoiceToQuestionnaireQuestionAsync(
		Guid questionnaireId,
		Guid questionId,
		string choice,
		CancellationToken cancellationToken = default)
	{
		var questionnaire = await questionnaireRepository.GetAsync(questionnaireId, cancellationToken);
		if (questionnaire.IsFailure) 
			return questionnaire.Error;

		var result = questionnaire.Value.AddChoiceToQuestion(questionId, choice);
		if (result.IsFailure)
			return result.Error;
		
		await unitOfWork.SaveChangesAsync(cancellationToken);

		return questionnaire;
	}
	
	public async Task<Result<Questionnaire>> RemoveChoiceFromQuestionnaireQuestionAsync(
		Guid questionnaireId,
		Guid questionId,
		Guid choiceId,
		CancellationToken cancellationToken = default)
	{
		var questionnaire = await questionnaireRepository.GetAsync(questionnaireId, cancellationToken);
		if (questionnaire.IsFailure) 
			return questionnaire.Error;

		var result = questionnaire.Value.RemoveChoiceFromQuestion(questionId, choiceId);
		if (result.IsFailure)
			return result.Error;
		
		await unitOfWork.SaveChangesAsync(cancellationToken);

		return questionnaire;
	}
}