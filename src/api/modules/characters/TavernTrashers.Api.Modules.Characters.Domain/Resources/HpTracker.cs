using TavernTrashers.Api.Common.Domain.Entities;
using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Modules.Characters.Domain.Resources;

public sealed class HpTracker : Entity
{
	private HpTracker() { }

	public Guid CharacterId { get; private set; }
	public int BaseMaxHp { get; private set; }
	public int MaxHpReduction { get; private set; }
	public int CurrentHp { get; private set; }
	public int TemporaryHp { get; private set; }

	public int EffectiveMaxHp => BaseMaxHp - MaxHpReduction;

	public static Result<HpTracker> Create(Guid characterId, int baseMaxHp = 0)
	{
		if (baseMaxHp < 0)
			return HpTrackerErrors.InvalidBaseMaxHp();

		var tracker = new HpTracker
		{
			Id = Guid.NewGuid(),
			CharacterId = characterId,
			BaseMaxHp = baseMaxHp,
			MaxHpReduction = 0,
			CurrentHp = baseMaxHp,
			TemporaryHp = 0,
		};

		return tracker;
	}

	public Result SetBaseMaxHp(int baseMaxHp)
	{
		if (baseMaxHp < 0)
			return HpTrackerErrors.InvalidBaseMaxHp();

		BaseMaxHp = baseMaxHp;

		// Adjust current HP if it exceeds the new effective max HP
		if (CurrentHp > EffectiveMaxHp)
			CurrentHp = EffectiveMaxHp;

		return Result.Success();
	}

	public Result TakeDamage(int damage)
	{
		if (damage < 0)
			return HpTrackerErrors.InvalidDamage();

		if (damage == 0)
			return Result.Success();

		// First exhaust temporary HP
		if (TemporaryHp > 0)
		{
			if (damage <= TemporaryHp)
			{
				TemporaryHp -= damage;
				return Result.Success();
			}

			// Damage exceeds temporary HP
			damage -= TemporaryHp;
			TemporaryHp = 0;
		}

		// Reduce current HP by remaining damage
		CurrentHp = Math.Max(0, CurrentHp - damage);

		return Result.Success();
	}

	public Result Heal(int healing)
	{
		if (healing < 0)
			return HpTrackerErrors.InvalidHealing();

		if (healing == 0)
			return Result.Success();

		// Cannot exceed effective max HP
		CurrentHp = Math.Min(EffectiveMaxHp, CurrentHp + healing);

		return Result.Success();
	}

	public Result SetTemporaryHp(int temporaryHp)
	{
		if (temporaryHp < 0)
			return HpTrackerErrors.InvalidTemporaryHp();

		// Take the higher value (no stacking)
		TemporaryHp = Math.Max(TemporaryHp, temporaryHp);

		return Result.Success();
	}

	public Result ApplyMaxHpReduction(int reduction)
	{
		if (reduction < 0)
			return HpTrackerErrors.InvalidMaxHpReduction();

		MaxHpReduction += reduction;

		// Adjust current HP if it exceeds the new effective max HP
		if (CurrentHp > EffectiveMaxHp)
			CurrentHp = EffectiveMaxHp;

		return Result.Success();
	}

	public Result RemoveMaxHpReduction(int reduction)
	{
		if (reduction < 0)
			return HpTrackerErrors.InvalidMaxHpReduction();

		MaxHpReduction = Math.Max(0, MaxHpReduction - reduction);

		return Result.Success();
	}

	// DM methods to directly set any HP field
	public Result SetCurrentHp(int currentHp)
	{
		if (currentHp < 0)
			return HpTrackerErrors.InvalidCurrentHp();

		CurrentHp = currentHp;

		return Result.Success();
	}

	public Result SetMaxHpReduction(int maxHpReduction)
	{
		if (maxHpReduction < 0)
			return HpTrackerErrors.InvalidMaxHpReduction();

		MaxHpReduction = maxHpReduction;

		// Adjust current HP if it exceeds the new effective max HP
		if (CurrentHp > EffectiveMaxHp)
			CurrentHp = EffectiveMaxHp;

		return Result.Success();
	}
}
