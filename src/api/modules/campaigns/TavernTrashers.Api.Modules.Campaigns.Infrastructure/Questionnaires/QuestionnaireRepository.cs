using Microsoft.EntityFrameworkCore;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Campaigns.Domain.Questionnaires;
using TavernTrashers.Api.Modules.Campaigns.Infrastructure.Database;

namespace TavernTrashers.Api.Modules.Campaigns.Infrastructure.Questionnaires;

internal sealed class QuestionnaireRepository(CampaignsDbContext dbContext) : IQuestionnaireRepository
{
	public async Task<IReadOnlyCollection<Questionnaire>> GetAllAsync(CancellationToken cancellationToken = default) =>
		await dbContext
		   .Questionnaires
		   .Include(questionnaire => questionnaire.Questions)
		   .ThenInclude(question => question.Choices)
		   .ToListAsync(cancellationToken);

	public async Task<Result<Questionnaire>> GetAsync(Guid questionnaireId, CancellationToken cancellationToken = default) =>
		await dbContext
		   .Questionnaires
		   .Include(questionnaire => questionnaire.Questions)
		   .ThenInclude(question => question.Choices)
		   .SingleOrDefaultAsync(questionnaire => questionnaire.Id == questionnaireId, cancellationToken)
		   .ToResultAsync(QuestionnaireErrors.NotFound(questionnaireId));

	public void Add(Questionnaire questionnaire)
	{
		dbContext.Questionnaires.Add(questionnaire);
	}
}