using TavernTrashers.Api.Common.Domain.Entities;
using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Modules.Characters.Domain.Resources;

public sealed class HpTracker : Entity
{
	private HpTracker() { }

	public Guid CharacterId { get; private set; }
	public int BaseMaxHp { get; private set; }
	public int CurrentHp { get; private set; }
	public int TemporaryHp { get; private set; }
	public int MaxHpReduction { get; private set; }

	public int EffectiveMaxHp => BaseMaxHp - MaxHpReduction;

	public static Result<HpTracker> Create(Guid characterId, int baseMaxHp)
	{
		if (baseMaxHp <= 0)
			return HpTrackerErrors.InvalidBaseMaxHp(baseMaxHp);

		return new HpTracker
		{
			Id             = Guid.NewGuid(),
			CharacterId    = characterId,
			BaseMaxHp      = baseMaxHp,
			CurrentHp      = baseMaxHp,
			TemporaryHp    = 0,
			MaxHpReduction = 0,
		};
	}

	public Result SetBaseMaxHp(int baseMaxHp)
	{
		if (baseMaxHp <= 0)
			return HpTrackerErrors.InvalidBaseMaxHp(baseMaxHp);

		BaseMaxHp = baseMaxHp;
		CurrentHp = Math.Min(CurrentHp, EffectiveMaxHp);
		return Result.Success();
	}

	public Result TakeDamage(int amount)
	{
		if (amount <= 0)
			return HpTrackerErrors.InvalidAmount(amount);

		var tempAbsorbed = Math.Min(TemporaryHp, amount);
		TemporaryHp -= tempAbsorbed;

		var overflow = amount - tempAbsorbed;
		CurrentHp   = Math.Max(0, CurrentHp - overflow);

		return Result.Success();
	}

	public Result Heal(int amount)
	{
		if (amount <= 0)
			return HpTrackerErrors.InvalidAmount(amount);

		CurrentHp = Math.Min(CurrentHp + amount, EffectiveMaxHp);
		return Result.Success();
	}

	public Result SetTemporaryHp(int amount)
	{
		if (amount < 0)
			return HpTrackerErrors.InvalidTemporaryHp(amount);

		TemporaryHp = Math.Max(TemporaryHp, amount);
		return Result.Success();
	}

	public Result ApplyMaxHpReduction(int reduction)
	{
		if (reduction <= 0)
			return HpTrackerErrors.InvalidMaxHpReduction(reduction);

		MaxHpReduction += reduction;
		CurrentHp      =  Math.Min(CurrentHp, EffectiveMaxHp);
		return Result.Success();
	}

	public void RemoveMaxHpReduction()
	{
		MaxHpReduction = 0;
	}

	public Result DirectSetCurrentHp(int hp)
	{
		if (hp < 0)
			return HpTrackerErrors.InvalidCurrentHp(hp);

		CurrentHp = Math.Min(hp, EffectiveMaxHp);
		return Result.Success();
	}

	public Result DirectSetTemporaryHp(int hp)
	{
		if (hp < 0)
			return HpTrackerErrors.InvalidTemporaryHp(hp);

		TemporaryHp = hp;
		return Result.Success();
	}

	public Result DirectSetBaseMaxHp(int baseMaxHp) => SetBaseMaxHp(baseMaxHp);

	public Result DirectSetMaxHpReduction(int reduction)
	{
		if (reduction < 0)
			return HpTrackerErrors.InvalidMaxHpReduction(reduction);

		MaxHpReduction = reduction;
		CurrentHp      = Math.Min(CurrentHp, EffectiveMaxHp);
		return Result.Success();
	}
}
