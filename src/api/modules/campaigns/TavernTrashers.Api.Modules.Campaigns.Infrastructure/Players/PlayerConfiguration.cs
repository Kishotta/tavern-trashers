using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TavernTrashers.Api.Modules.Campaigns.Domain.Players;

namespace TavernTrashers.Api.Modules.Campaigns.Infrastructure.Players;

public class PlayerConfiguration : IEntityTypeConfiguration<Player>
{
	public void Configure(EntityTypeBuilder<Player> builder) =>
		builder.HasKey(player => player.Id);
}