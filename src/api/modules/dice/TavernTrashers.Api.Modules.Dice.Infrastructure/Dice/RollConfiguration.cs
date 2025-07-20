using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;
using TavernTrashers.Api.Modules.Dice.Domain.Rolls;

namespace TavernTrashers.Api.Modules.Dice.Infrastructure.Dice;

internal sealed class RollConfiguration : IEntityTypeConfiguration<Roll>
{
	public void Configure(EntityTypeBuilder<Roll> builder)
	{
		builder.HasKey(roll => roll.Id);
		builder.HasOne(roll => roll.Parent)
		   .WithMany(roll => roll.Children)
		   .OnDelete(DeleteBehavior.Restrict);
		builder.Property(roll => roll.Expression).IsRequired();

		var dieResultListConverter = new ValueConverter<IReadOnlyList<DieResult>, string>(
			v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
			v => JsonSerializer.Deserialize<List<DieResult>>(v, (JsonSerializerOptions)null!) ?? new List<DieResult>()
		);

		builder.Property(roll => roll.RawRolls)
			.IsRequired()
			.HasConversion(dieResultListConverter)
			.HasColumnType("jsonb");
		builder.Property(roll => roll.KeptRolls)
			.IsRequired()
			.HasConversion(dieResultListConverter)
			.HasColumnType("jsonb");
		builder.Property(roll => roll.ContextJson).IsRequired().HasColumnType("jsonb");
	}
}