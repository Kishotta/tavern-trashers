using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns;

namespace TavernTrashers.Api.Modules.Campaigns.Infrastructure.Campaigns;

public class CampaignConfiguration : IEntityTypeConfiguration<Campaign>
{
	public void Configure(EntityTypeBuilder<Campaign> builder)
	{
		builder.HasKey(campaign => campaign.Id);

		builder.OwnsMany(campaign => campaign.Members, memberBuilder =>
		{
			memberBuilder.ToTable("campaign_members");
			memberBuilder.WithOwner().HasForeignKey("campaign_id");
			memberBuilder.HasKey("campaign_id", nameof(CampaignMember.PlayerId));

			memberBuilder.Property(member => member.Role).HasConversion<string>();
			memberBuilder.Property(member => member.Status).HasConversion<string>();
		});
	}
}