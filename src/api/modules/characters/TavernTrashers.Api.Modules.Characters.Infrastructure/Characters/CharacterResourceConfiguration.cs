using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TavernTrashers.Api.Modules.Characters.Domain.Resources;

namespace TavernTrashers.Api.Modules.Characters.Infrastructure.Characters;

internal sealed class CharacterResourceConfiguration : IEntityTypeConfiguration<CharacterResource>
{
	public void Configure(EntityTypeBuilder<CharacterResource> builder)
	{
		builder.HasKey(r => r.Id);
		builder.Property(r => r.CurrentAmount).IsRequired();
		builder.Property(r => r.MaxAmount).IsRequired();

		builder.HasOne(r => r.ResourceDefinition)
		   .WithMany()
		   .HasForeignKey(r => r.ResourceDefinitionId)
		   .OnDelete(DeleteBehavior.Restrict);

		builder.HasIndex(r => new { r.CharacterId, r.ResourceDefinitionId }).IsUnique();
	}
}
