using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns;

namespace TavernTrashers.Api.Modules.Campaigns.Infrastructure;

public class CampaignConfiguration : IEntityTypeConfiguration<Campaign>
{
	public void Configure(EntityTypeBuilder<Campaign> builder)
	{
		builder.HasKey(campaign => campaign.Id);

		builder.Property(campaign => campaign.Id)
		   .HasConversion<CampaignIdConverter>()
		   .ValueGeneratedNever();
	}
}

public class CampaignIdConverter() 
	: ValueConverter<CampaignId, Guid>(id => id, value => value);