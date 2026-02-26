using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TavernTrashers.Api.Modules.Characters.Domain.Classes;

namespace TavernTrashers.Api.Modules.Characters.Infrastructure.Classes;

internal sealed class ClassConfiguration : IEntityTypeConfiguration<Class>
{
	public void Configure(EntityTypeBuilder<Class> builder)
	{
		builder.HasKey(c => c.Id);
		builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
		builder.Property(c => c.DeletedAt);

		builder.HasIndex(c => c.Name).IsUnique();
	}
}
