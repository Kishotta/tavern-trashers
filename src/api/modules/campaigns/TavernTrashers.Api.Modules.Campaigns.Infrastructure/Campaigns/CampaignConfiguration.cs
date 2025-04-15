using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns;

namespace TavernTrashers.Api.Modules.Campaigns.Infrastructure.Campaigns;

public class CampaignConfiguration : IEntityTypeConfiguration<Campaign>
{
	public void Configure(EntityTypeBuilder<Campaign> builder)
	{
		builder.HasKey(campaign => campaign.Id);

		builder.HasMany(campaign => campaign.DungeonMasters)
		   .WithMany();

		builder
		   .Property(campaign => campaign.Status)
		   .HasConversion<string>();
	}
}