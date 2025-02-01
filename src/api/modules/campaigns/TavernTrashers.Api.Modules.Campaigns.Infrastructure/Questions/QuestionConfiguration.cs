using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TavernTrashers.Api.Modules.Campaigns.Domain.Questions;

namespace TavernTrashers.Api.Modules.Campaigns.Infrastructure.Questions;

internal sealed class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
	public void Configure(EntityTypeBuilder<Question> builder)
	{
		builder.HasKey(x => x.Id);

		builder.HasMany(question => question.Choices).WithOne();
	}
}