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

		builder.HasMany(c => c.ClassLevels)
		   .WithOne()
		   .HasForeignKey(cl => cl.CharacterId)
		   .OnDelete(DeleteBehavior.Cascade);

		builder.HasMany(c => c.Resources)
		   .WithOne()
		   .HasForeignKey(r => r.CharacterId)
		   .OnDelete(DeleteBehavior.Cascade);
	}
}
