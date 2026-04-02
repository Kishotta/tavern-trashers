using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TavernTrashers.Api.Modules.Characters.Domain.Resources;

namespace TavernTrashers.Api.Modules.Characters.Infrastructure.Characters;

internal sealed class SpellSlotLevelConfiguration : IEntityTypeConfiguration<SpellSlotLevel>
{
	public void Configure(EntityTypeBuilder<SpellSlotLevel> builder)
	{
		builder.HasKey(l => l.Id);
		builder.Property(l => l.Level).IsRequired();
		builder.Property(l => l.CurrentUses).IsRequired();
		builder.Property(l => l.MaxUses).IsRequired();
	}
}
