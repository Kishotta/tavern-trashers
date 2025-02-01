using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TavernTrashers.Api.Modules.Campaigns.Domain.Choices;

namespace TavernTrashers.Api.Modules.Campaigns.Infrastructure.Choices;

internal sealed class ChoiceConfiguration : IEntityTypeConfiguration<Choice>
{
	public void Configure(EntityTypeBuilder<Choice> builder)
	{
		builder.HasKey(x => x.Id);
	}
}