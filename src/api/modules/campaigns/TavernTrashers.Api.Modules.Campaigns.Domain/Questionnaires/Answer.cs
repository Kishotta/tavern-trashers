using TavernTrashers.Api.Modules.Campaigns.Domain.Characters;
using TavernTrashers.Api.Modules.Campaigns.Domain.Choices;
using TavernTrashers.Api.Modules.Campaigns.Domain.Questions;

namespace TavernTrashers.Api.Modules.Campaigns.Domain.Questionnaires;

public class Answer
{
	public Guid Id { get; set; }

	public Question Question { get; set; } = default!;
	
	public Character Character  { get; set; } = default!;
	
	public string? Text { get; private set; } = string.Empty;
	public Choice? Choice { get; private set; } = default!;
	
	private Answer() {}
	
	public static Answer Create(Question question, Character character, string? text, Choice? choice)
	{
		var answer = new Answer
		{
			Question  = question,
			Character = character,
			Text      = text,
			Choice    = choice
		};
		
		return answer;
	}
}