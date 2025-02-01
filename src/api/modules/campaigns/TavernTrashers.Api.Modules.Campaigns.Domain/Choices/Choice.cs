using TavernTrashers.Api.Modules.Campaigns.Domain.Questions;

namespace TavernTrashers.Api.Modules.Campaigns.Domain.Choices;

public class Choice
{
	public Guid Id { get; private set; }
	
	public Guid QuestionId { get; private set; }
	
	public string Text { get; private set; } = string.Empty;
	
	private Choice() {}
	
	public static Choice Create(Question question, string text)
	{
		var choice = new Choice
		{
			Id         = Guid.NewGuid(),
			QuestionId = question.Id,
			Text       = text
		};
		
		return choice;
	}
}