using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TavernTrashers.Api.Modules.Characters.Domain.Resources;

namespace TavernTrashers.Api.Modules.Characters.Infrastructure.Characters;

internal sealed class HpTrackerConfiguration : IEntityTypeConfiguration<HpTracker>
{
	public void Configure(EntityTypeBuilder<HpTracker> builder)
	{
		builder.HasKey(h => h.Id);
		builder.Property(h => h.BaseMaxHp).IsRequired();
		builder.Property(h => h.CurrentHp).IsRequired();
		builder.Property(h => h.TemporaryHp).IsRequired();
		builder.Property(h => h.MaxHpReduction).IsRequired();

		builder.Ignore(h => h.EffectiveMaxHp);
	}
}
