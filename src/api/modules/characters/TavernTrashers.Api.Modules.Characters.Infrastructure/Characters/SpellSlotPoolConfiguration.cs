using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TavernTrashers.Api.Modules.Characters.Domain.Resources;

namespace TavernTrashers.Api.Modules.Characters.Infrastructure.Characters;

internal sealed class SpellSlotPoolConfiguration : IEntityTypeConfiguration<SpellSlotPool>
{
	public void Configure(EntityTypeBuilder<SpellSlotPool> builder)
	{
		builder.HasKey(p => p.Id);
		builder.Property(p => p.Kind).IsRequired().HasConversion<string>();

		builder.HasMany(p => p.Levels)
		   .WithOne()
		   .HasForeignKey(l => l.SpellSlotPoolId)
		   .OnDelete(DeleteBehavior.Cascade);
	}
}
