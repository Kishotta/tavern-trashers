using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Modules.Campaigns.Domain.Questionnaires;

public interface IQuestionnaireRepository
{
	Task<IReadOnlyCollection<Questionnaire>> GetAllAsync(CancellationToken cancellationToken = default);
	Task<Result<Questionnaire>> GetAsync(Guid questionnaireId, CancellationToken cancellationToken = default);
	void Add(Questionnaire questionnaire);
	
}