using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TavernTrashers.Api.Modules.Characters.Domain.Resources;

namespace TavernTrashers.Api.Modules.Characters.Infrastructure.Classes;

internal sealed class ResourceDefinitionConfiguration : IEntityTypeConfiguration<ResourceDefinition>
{
	public void Configure(EntityTypeBuilder<ResourceDefinition> builder)
	{
		builder.HasKey(rd => rd.Id);
		builder.Property(rd => rd.Name).IsRequired().HasMaxLength(100);

		builder.OwnsMany(rd => rd.Allowances, allowance =>
		{
			allowance.WithOwner().HasForeignKey("ResourceDefinitionId");
			allowance.Property(a => a.Level).IsRequired();
			allowance.Property(a => a.Amount).IsRequired();
			allowance.HasKey("ResourceDefinitionId", nameof(ResourceAllowance.Level));
		});
	}
}