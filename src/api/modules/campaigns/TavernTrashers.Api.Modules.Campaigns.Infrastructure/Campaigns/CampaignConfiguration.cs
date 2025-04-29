using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TavernTrashers.Api.Modules.Campaigns.Domain.Campaigns;

namespace TavernTrashers.Api.Modules.Campaigns.Infrastructure.Campaigns;

public class CampaignConfiguration : IEntityTypeConfiguration<Campaign>
{
	public void Configure(EntityTypeBuilder<Campaign> builder)
	{
		builder.HasKey(campaign => campaign.Id);

		builder.OwnsMany(
			campaign => campaign.Invitations,
			invitationBuilder =>
			{
				invitationBuilder.ToTable("campaign_invitations");

				invitationBuilder
				   .Property(invitation => invitation.Role)
				   .HasConversion<string>();
			});
	}
}