using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TavernTrashers.Api.Modules.Characters.Domain.Campaigns;

namespace TavernTrashers.Api.Modules.Characters.Infrastructure.Campaigns;

internal sealed class CampaignReadModelConfiguration : IEntityTypeConfiguration<CampaignReadModel>
{
	public void Configure(EntityTypeBuilder<CampaignReadModel> builder)
	{
		builder.HasKey(c => c.Id);
		builder.Property(c => c.Title).IsRequired().HasMaxLength(200);
	}
}
