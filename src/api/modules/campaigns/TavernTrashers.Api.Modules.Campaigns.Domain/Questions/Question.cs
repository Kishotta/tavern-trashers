using TavernTrashers.Api.Modules.Campaigns.Domain.Choices;
using TavernTrashers.Api.Modules.Campaigns.Domain.Questionnaires;

namespace TavernTrashers.Api.Modules.Campaigns.Domain.Questions;

public class Question : Entity
{
	public Guid QuestionnaireId { get; private set; }
	
	public string Prompt { get; private set; } = string.Empty;
	public QuestionType Type { get; private set; }
	
	public  IReadOnlyCollection<Choice> Choices => _choices.AsReadOnly();
	private readonly List<Choice> _choices = [];
	
	private Question() {}
	
	internal static Question Create(Questionnaire questionnaire, string prompt, QuestionType type)
	{
		var question = new Question
		{
			Id            = Guid.NewGuid(),
			QuestionnaireId = questionnaire.Id,
			Prompt        = prompt,
			Type          = type
		};
		
		return question;
	}
	
	internal Choice AddChoice(string choiceText)
	{
		var choice = Choice.Create(this, choiceText);
		
		_choices.Add(choice);

		return choice;
	}
	
	internal void RemoveChoice(Choice choice)
	{
		_choices.Remove(choice);
	}
}