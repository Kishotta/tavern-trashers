using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TavernTrashers.Api.Modules.Characters.Domain.Characters;

namespace TavernTrashers.Api.Modules.Characters.Infrastructure.Characters;

internal sealed class CharacterConfiguration : IEntityTypeConfiguration<Character>
{
	public void Configure(EntityTypeBuilder<Character> builder)
	{
		builder.HasKey(c => c.Id);
		builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
		builder.Property(c => c.Level).IsRequired();
		builder.Property(c => c.OwnerId).IsRequired();
		builder.Property(c => c.CampaignId).IsRequired();

		builder.HasIndex(c => c.CampaignId);

		builder.HasOne(c => c.HpTracker)
		   .WithOne()
		   .HasForeignKey<Domain.Resources.HpTracker>(h => h.CharacterId)
		   .OnDelete(DeleteBehavior.Cascade);

		builder.HasMany(c => c.ClassLevels)
		   .WithOne()
		   .HasForeignKey(cl => cl.CharacterId)
		   .OnDelete(DeleteBehavior.Cascade);

		builder.HasMany(c => c.Resources)
		   .WithOne()
		   .HasForeignKey(r => r.CharacterId)
		   .OnDelete(DeleteBehavior.Cascade);

		builder.HasMany(c => c.GenericResources)
		   .WithOne()
		   .HasForeignKey(r => r.CharacterId)
		   .OnDelete(DeleteBehavior.Cascade);
	}
}
