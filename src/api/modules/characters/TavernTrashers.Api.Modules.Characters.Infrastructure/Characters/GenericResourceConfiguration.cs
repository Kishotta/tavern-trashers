using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TavernTrashers.Api.Modules.Characters.Domain.Resources;

namespace TavernTrashers.Api.Modules.Characters.Infrastructure.Characters;

internal sealed class GenericResourceConfiguration : IEntityTypeConfiguration<GenericResource>
{
	public void Configure(EntityTypeBuilder<GenericResource> builder)
	{
		builder.HasKey(r => r.Id);
		builder.Property(r => r.Name).IsRequired().HasMaxLength(100);
		builder.Property(r => r.SourceCategory).HasMaxLength(100);
		builder.Property(r => r.MaxAmount).IsRequired();
		builder.Property(r => r.CurrentAmount).IsRequired();
		builder.Property(r => r.Direction).IsRequired();
		builder.Property(r => r.ResetTriggers).IsRequired();
	}
}
