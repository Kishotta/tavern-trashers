using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TavernTrashers.Api.Modules.Dice.Domain.Rolls;

namespace TavernTrashers.Api.Modules.Dice.Infrastructure.Dice;

internal sealed class RollConfiguration : IEntityTypeConfiguration<Roll>
{
	public void Configure(EntityTypeBuilder<Roll> builder)
	{
		builder.HasKey(roll => roll.Id);
		builder.Property(roll => roll.Expression).IsRequired();
		builder.Property(roll => roll.RawRolls).IsRequired().HasColumnType("integer[]");
		builder.Property(roll => roll.KeptRolls).IsRequired().HasColumnType("integer[]");
		builder.Property(roll => roll.ContextJson).IsRequired().HasColumnType("jsonb");
	}
}