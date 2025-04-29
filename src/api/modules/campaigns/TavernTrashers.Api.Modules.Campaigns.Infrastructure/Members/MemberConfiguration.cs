using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TavernTrashers.Api.Modules.Campaigns.Domain.Members;

namespace TavernTrashers.Api.Modules.Campaigns.Infrastructure.Members;

public class MemberConfiguration : IEntityTypeConfiguration<Member>
{
	public void Configure(EntityTypeBuilder<Member> builder) =>
		builder.HasKey(player => player.Id);
}