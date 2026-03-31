using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TavernTrashers.Api.Modules.Characters.Domain.Resources;

namespace TavernTrashers.Api.Modules.Characters.Infrastructure.Characters;

internal sealed class HitPointsConfiguration : IEntityTypeConfiguration<HitPoints>
{
	public void Configure(EntityTypeBuilder<HitPoints> builder)
	{
		builder.HasKey(h => h.Id);
		builder.Property(h => h.BaseMaxHitPoints).IsRequired();
		builder.Property(h => h.CurrentHitPoints).IsRequired();
		builder.Property(h => h.TemporaryHitPoints).IsRequired();
		builder.Property(h => h.MaxHitPointReduction).IsRequired();

		builder.Ignore(h => h.EffectiveMaxHitPoints);
	}
}
