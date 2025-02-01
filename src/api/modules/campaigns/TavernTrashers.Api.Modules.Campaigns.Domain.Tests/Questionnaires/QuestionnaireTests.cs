using Shouldly;
using TavernTrashers.Api.Common.Testing;
using TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns;
using TavernTrashers.Api.Modules.Campaigns.Domain.Choices;
using TavernTrashers.Api.Modules.Campaigns.Domain.Choices.DomainEvents;
using TavernTrashers.Api.Modules.Campaigns.Domain.Questionnaires;
using TavernTrashers.Api.Modules.Campaigns.Domain.Questionnaires.DomainEvents;
using TavernTrashers.Api.Modules.Campaigns.Domain.Questions;
using TavernTrashers.Api.Modules.Campaigns.Domain.Questions.DomainEvents;

namespace TavernTrashers.Api.Modules.Campaigns.Domain.Tests.Questionnaires;

public class QuestionnaireTests : TestBase
{
	private readonly Campaign _sampleCampaign;
	private readonly Questionnaire _sampleQuestionnaire;

	public QuestionnaireTests()
	{
		_sampleCampaign = Campaign.Create("Test Campaign", "Test Campaign Description");
		_sampleQuestionnaire = Questionnaire.Create(_sampleCampaign, "Test Title", "Test Description");
	}
	
	[Fact]
	public void Create_ShouldRaiseDomainEvent_WhenCreated()
	{
		var questionnaire = Questionnaire.Create(_sampleCampaign, "Test Title", "Test Description");

		
		questionnaire.Campaign.Id.ShouldBe(_sampleCampaign.Id);
		questionnaire.Title.ShouldBe("Test Title");
		questionnaire.Description.ShouldBe("Test Description");
		
		var domainEvent = AssertDomainEventWasPublished<QuestionnaireCreatedDomainEvent>(questionnaire);
		domainEvent.QuestionnaireId.ShouldBe(questionnaire.Id);
	}
	
	[Fact]
	public void AddQuestion_ShouldRaiseDomainEvent_WhenQuestionAdded()
	{
		var question = _sampleQuestionnaire.AddQuestion("Test Question", QuestionType.Text);

		
		_sampleQuestionnaire.Questions.Count.ShouldBe(1);
		
		question.IsSuccess.ShouldBeTrue();
		question.Value.QuestionnaireId.ShouldBe(_sampleQuestionnaire.Id);
		question.Value.Prompt.ShouldBe("Test Question");
		question.Value.Type.ShouldBe(QuestionType.Text);
		
		var domainEvent = AssertDomainEventWasPublished<QuestionAddedDomainEvent>(_sampleQuestionnaire);
		domainEvent.QuestionnaireId.ShouldBe(_sampleQuestionnaire.Id);
		domainEvent.QuestionId.ShouldBe(question.Value.Id);
	}
	
	[Fact]
	public void AddQuestion_ShouldReturnFailureResult_WhenQuestionNotAdded()
	{
		_sampleQuestionnaire.AddQuestion("Test Question", QuestionType.Text);
		var duplicateQuestion = _sampleQuestionnaire.AddQuestion("Test Question", QuestionType.Text);
		
		
		duplicateQuestion.IsFailure.ShouldBeTrue();
		duplicateQuestion.Error.ShouldBe(QuestionErrors.AlreadyExists);
	}
	
	[Fact]
	public void RemoveQuestion_ShouldRaiseDomainEvent_WhenQuestionRemoved()
	{
		var question = _sampleQuestionnaire.AddQuestion("Test Question", QuestionType.Text).Value;

		
		_sampleQuestionnaire.RemoveQuestion(question.Id);

		
		_sampleQuestionnaire.Questions.Count.ShouldBe(0);
		
		var domainEvent = AssertDomainEventWasPublished<QuestionRemovedDomainEvent>(_sampleQuestionnaire);
		domainEvent.QuestionnaireId.ShouldBe(_sampleQuestionnaire.Id);
		domainEvent.QuestionId.ShouldBe(question.Id);
	}
	
	[Fact]
	public void RemoveQuestion_ShouldReturnFailureResult_WhenQuestionNotRemoved()
	{
		_sampleQuestionnaire.AddQuestion("Test Question", QuestionType.Text);


		var questionId = Guid.NewGuid();
		var result     = _sampleQuestionnaire.RemoveQuestion(questionId);

		
		result.IsFailure.ShouldBeTrue();
		result.Error.ShouldBe(QuestionErrors.NotFound(questionId));
	}
	
	[Fact]
	public void AddChoiceToQuestion_ShouldRaiseDomainEvent_WhenChoiceAddedToQuestion()
	{
		var question = _sampleQuestionnaire.AddQuestion("Test Question", QuestionType.SingleChoice).Value;

		
		var choice = _sampleQuestionnaire.AddChoiceToQuestion(question.Id, "Test Choice");

		
		question.Choices.Count.ShouldBe(1);
		
		choice.IsSuccess.ShouldBeTrue();
		choice.Value.QuestionId.ShouldBe(question.Id);
		choice.Value.Text.ShouldBe("Test Choice");
		
		var domainEvent = AssertDomainEventWasPublished<ChoiceAddedDomainEvent>(_sampleQuestionnaire);
		domainEvent.QuestionnaireId.ShouldBe(_sampleQuestionnaire.Id);
		domainEvent.QuestionId.ShouldBe(question.Id);
		domainEvent.ChoiceId.ShouldBe(choice.Value.Id);
	}
	
	[Fact]
	public void AddChoiceToQuestion_ShouldReturnFailureResult_WhenChoiceNotAddedToQuestion()
	{
		var question = _sampleQuestionnaire.AddQuestion("Test Question", QuestionType.SingleChoice).Value;

		
		_sampleQuestionnaire.AddChoiceToQuestion(question.Id, "Test Choice");
		var duplicateChoice = _sampleQuestionnaire.AddChoiceToQuestion(question.Id, "Test Choice");

		
		duplicateChoice.IsFailure.ShouldBeTrue();
		duplicateChoice.Error.ShouldBe(ChoiceErrors.AlreadyExists);
	}
	
	[Fact]
	public void AddChoiceToQuestion_ShouldReturnFailureResult_WhenQuestionNotFound()
	{
		_sampleQuestionnaire.AddQuestion("Test Question", QuestionType.SingleChoice);

		
		var questionId = Guid.NewGuid();
		var result     = _sampleQuestionnaire.AddChoiceToQuestion(questionId, "Test Choice");

		
		result.IsFailure.ShouldBeTrue();
		result.Error.ShouldBe(QuestionErrors.NotFound(questionId));
	}
	
	[Fact]
	public void RemoveChoiceFromQuestion_ShouldRaiseDomainEvent_WhenChoiceRemovedFromQuestion()
	{
		var question = _sampleQuestionnaire.AddQuestion("Test Question", QuestionType.SingleChoice).Value;
		var choice = _sampleQuestionnaire.AddChoiceToQuestion(question.Id, "Test Choice").Value;

		
		_sampleQuestionnaire.RemoveChoiceFromQuestion(question.Id, choice.Id);

		
		question.Choices.Count.ShouldBe(0);
		
		var domainEvent = AssertDomainEventWasPublished<ChoiceRemovedDomainEvent>(_sampleQuestionnaire);
		domainEvent.QuestionnaireId.ShouldBe(_sampleQuestionnaire.Id);
		domainEvent.QuestionId.ShouldBe(question.Id);
		domainEvent.ChoiceId.ShouldBe(choice.Id);
	}
	
	[Fact]
	public void RemoveChoiceFromQuestion_ShouldReturnFailureResult_WhenChoiceNotFound()
	{
		var question = _sampleQuestionnaire.AddQuestion("Test Question", QuestionType.SingleChoice).Value;
		_sampleQuestionnaire.AddChoiceToQuestion(question.Id, "Test Choice");

		
		var choiceId = Guid.NewGuid();
		var result   = _sampleQuestionnaire.RemoveChoiceFromQuestion(question.Id, choiceId);

		
		result.IsFailure.ShouldBeTrue();
		result.Error.ShouldBe(ChoiceErrors.NotFound(choiceId));
	}
	
	[Fact]
	public void RemoveChoiceFromQuestion_ShouldReturnFailureResult_WhenQuestionNotFound()
	{
		var question = _sampleQuestionnaire.AddQuestion("Test Question", QuestionType.SingleChoice);
		var choice = _sampleQuestionnaire.AddChoiceToQuestion(question.Value.Id, "Test Choice").Value;

		
		var questionId = Guid.NewGuid();
		var result = _sampleQuestionnaire.RemoveChoiceFromQuestion(questionId, choice.Id);

		
		result.IsFailure.ShouldBeTrue();
		result.Error.ShouldBe(QuestionErrors.NotFound(questionId));
	}
}