using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TavernTrashers.Api.Modules.Characters.Domain.Classes;

namespace TavernTrashers.Api.Modules.Characters.Infrastructure.Classes;

internal sealed class CharacterClassConfiguration : IEntityTypeConfiguration<CharacterClass>
{
	public void Configure(EntityTypeBuilder<CharacterClass> builder)
	{
		builder.HasKey(c => c.Id);
		builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
		builder.Property(c => c.IsHomebrew).IsRequired();

		builder.HasMany(c => c.ResourceDefinitions)
		   .WithOne()
		   .HasForeignKey(rd => rd.CharacterClassId)
		   .OnDelete(DeleteBehavior.Cascade);

		builder.HasIndex(c => c.Name).IsUnique();
	}
}