using TavernTrashers.Api.Common.Domain.Auditing;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns;
using TavernTrashers.Api.Modules.Campaigns.Domain.Choices;
using TavernTrashers.Api.Modules.Campaigns.Domain.Choices.DomainEvents;
using TavernTrashers.Api.Modules.Campaigns.Domain.Questionnaires.DomainEvents;
using TavernTrashers.Api.Modules.Campaigns.Domain.Questions;
using TavernTrashers.Api.Modules.Campaigns.Domain.Questions.DomainEvents;

namespace TavernTrashers.Api.Modules.Campaigns.Domain.Questionnaires;

[Auditable]
public class Questionnaire : Entity
{
	public Campaign Campaign { get; private set; } = default!;
	
	public string Title { get; private set; } = string.Empty;
	public string Description { get; private set; } = string.Empty;
	
	public IReadOnlyCollection<Question> Questions => _questions.AsReadOnly();
	private readonly List<Question> _questions = [];
	
	private Questionnaire() {}
	
	public static Questionnaire Create(Campaign campaign, string title, string description)
	{
		var questionnaire = new Questionnaire
		{
			Id          = Guid.NewGuid(),
			Campaign    = campaign,
			Title       = title,
			Description = description
		};
		
		questionnaire.RaiseDomainEvent(new QuestionnaireCreatedDomainEvent(questionnaire.Id));
		
		return questionnaire;
	}
	
	public Result<Question> AddQuestion(string prompt, QuestionType type)
	{
		var existingQuestion = _questions.SingleOrDefault(question => question.Prompt == prompt);
		if (existingQuestion is not null)
			return QuestionErrors.AlreadyExists;
		
		var question = Question.Create(this, prompt, type);
		
		_questions.Add(question);
		
		RaiseDomainEvent(new QuestionAddedDomainEvent(Id, question.Id));

		return question;
	}
	
	public Result RemoveQuestion(Guid questionId)
	{
		var question = _questions.SingleOrDefault(question => question.Id == questionId);
		if (question is null)
			return QuestionErrors.NotFound(questionId);
		
		_questions.Remove(question);
		
		RaiseDomainEvent(new QuestionRemovedDomainEvent(Id, question.Id));

		return Unit.Value;
	}
	
	public Result<Choice> AddChoiceToQuestion(Guid questionId, string choiceText)
	{
		var question = _questions.SingleOrDefault(question => question.Id == questionId);
		if (question is null)
			return QuestionErrors.NotFound(questionId);
		
		var existingChoice = question.Choices.SingleOrDefault(choice => choice.Text == choiceText);
		if (existingChoice is not null)
			return ChoiceErrors.AlreadyExists;
		
		var choice = question.AddChoice(choiceText);
		
		RaiseDomainEvent(new ChoiceAddedDomainEvent(Id, question.Id, choice.Id));

		return choice;
	}
	
	public Result RemoveChoiceFromQuestion(Guid questionId, Guid choiceId)
	{
		var question = _questions.SingleOrDefault(question => question.Id == questionId);
		if (question is null)
			return QuestionErrors.NotFound(questionId);
		
		var choice = question.Choices.SingleOrDefault(choice => choice.Id == choiceId);
		if (choice is null)
			return ChoiceErrors.NotFound(choiceId);
		
		question.RemoveChoice(choice);
		
		RaiseDomainEvent(new ChoiceRemovedDomainEvent(Id, question.Id, choice.Id));
		
		return Unit.Value;
	}
}