using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TavernTrashers.Api.Modules.Characters.Domain.Resources;

namespace TavernTrashers.Api.Modules.Characters.Infrastructure.Characters;

internal sealed class DeathSavingThrowsConfiguration : IEntityTypeConfiguration<DeathSavingThrows>
{
	public void Configure(EntityTypeBuilder<DeathSavingThrows> builder)
	{
		builder.HasKey(d => d.Id);
		builder.Property(d => d.Successes).IsRequired();
		builder.Property(d => d.Failures).IsRequired();
	}
}
