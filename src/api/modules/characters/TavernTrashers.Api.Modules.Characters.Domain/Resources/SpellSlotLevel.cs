using TavernTrashers.Api.Common.Domain.Entities;
using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Modules.Characters.Domain.Resources;

public sealed class SpellSlotLevel : Entity
{
	private SpellSlotLevel() { }

	public Guid SpellSlotPoolId { get; private set; }
	public int Level { get; private set; }
	public int CurrentUses { get; private set; }
	public int MaxUses { get; private set; }

	internal static SpellSlotLevel CreateDefault(Guid poolId, int level) =>
		new()
		{
			Id             = Guid.NewGuid(),
			SpellSlotPoolId = poolId,
			Level          = level,
			CurrentUses    = 0,
			MaxUses        = 0,
		};

	internal Result Use()
	{
		if (CurrentUses <= 0)
			return SpellSlotPoolErrors.LevelEmpty(Level);

		CurrentUses--;
		return Result.Success();
	}

	internal void Restore() => CurrentUses = MaxUses;

	internal Result SetMax(int max)
	{
		if (max < 0)
			return SpellSlotPoolErrors.InvalidMaxUses(max);

		CurrentUses = max > MaxUses ? max : Math.Min(CurrentUses, max);
		MaxUses     = max;
		return Result.Success();
	}
}
