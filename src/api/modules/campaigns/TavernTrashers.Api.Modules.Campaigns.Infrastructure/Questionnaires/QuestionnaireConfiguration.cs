using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TavernTrashers.Api.Modules.Campaigns.Domain.Questionnaires;

namespace TavernTrashers.Api.Modules.Campaigns.Infrastructure.Questionnaires;

internal sealed class QuestionnaireConfiguration : IEntityTypeConfiguration<Questionnaire>
{
	public void Configure(EntityTypeBuilder<Questionnaire> builder)
	{
		builder.HasKey(x => x.Id);

		builder.HasMany(questionnaire => questionnaire.Questions).WithOne();
	}
}