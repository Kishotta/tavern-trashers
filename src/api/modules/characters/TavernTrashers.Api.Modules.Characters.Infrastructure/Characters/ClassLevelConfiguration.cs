using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TavernTrashers.Api.Modules.Characters.Domain.ClassLevels;

namespace TavernTrashers.Api.Modules.Characters.Infrastructure.Characters;

internal sealed class ClassLevelConfiguration : IEntityTypeConfiguration<ClassLevel>
{
	public void Configure(EntityTypeBuilder<ClassLevel> builder)
	{
		builder.HasKey(cl => cl.Id);
		builder.Property(cl => cl.Level).IsRequired();

		builder.HasOne(cl => cl.CharacterClass)
		   .WithMany()
		   .HasForeignKey(cl => cl.CharacterClassId)
		   .OnDelete(DeleteBehavior.Restrict);

		builder.HasIndex(cl => new { cl.CharacterId, cl.CharacterClassId }).IsUnique();
	}
}
