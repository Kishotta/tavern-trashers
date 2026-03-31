using TavernTrashers.Api.Common.Domain.Entities;
using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Modules.Characters.Domain.Resources;

public sealed class HitPoints : Entity
{
	private HitPoints() { }

	public Guid CharacterId { get; private set; }
	public int BaseMaxHitPoints { get; private set; }
	public int CurrentHitPoints { get; private set; }
	public int TemporaryHitPoints { get; private set; }
	public int MaxHitPointReduction { get; private set; }

	public int EffectiveMaxHitPoints => BaseMaxHitPoints - MaxHitPointReduction;

	public static HitPoints CreateDefault(Guid characterId) =>
		new()
		{
			Id                   = Guid.NewGuid(),
			CharacterId          = characterId,
			BaseMaxHitPoints     = 0,
			CurrentHitPoints     = 0,
			TemporaryHitPoints   = 0,
			MaxHitPointReduction = 0,
		};

	public static Result<HitPoints> Create(Guid characterId, int baseMaxHitPoints)
	{
		if (baseMaxHitPoints < 0)
			return HitPointsErrors.InvalidBaseMaxHitPoints(baseMaxHitPoints);

		return new HitPoints
		{
			Id                   = Guid.NewGuid(),
			CharacterId          = characterId,
			BaseMaxHitPoints     = baseMaxHitPoints,
			CurrentHitPoints     = baseMaxHitPoints,
			TemporaryHitPoints   = 0,
			MaxHitPointReduction = 0,
		};
	}

	public Result SetBaseMaxHitPoints(int baseMaxHitPoints)
	{
		if (baseMaxHitPoints < 0)
			return HitPointsErrors.InvalidBaseMaxHitPoints(baseMaxHitPoints);

		BaseMaxHitPoints = baseMaxHitPoints;
		CurrentHitPoints = Math.Min(CurrentHitPoints, EffectiveMaxHitPoints);
		return Result.Success();
	}

	public Result TakeDamage(int amount)
	{
		if (amount < 0)
			return HitPointsErrors.InvalidAmount(amount);

		var tempAbsorbed = Math.Min(TemporaryHitPoints, amount);
		TemporaryHitPoints -= tempAbsorbed;

		var overflow = amount - tempAbsorbed;
		CurrentHitPoints = Math.Max(0, CurrentHitPoints - overflow);

		return Result.Success();
	}

	public Result Heal(int amount)
	{
		if (amount < 0)
			return HitPointsErrors.InvalidAmount(amount);

		CurrentHitPoints = Math.Min(CurrentHitPoints + amount, EffectiveMaxHitPoints);
		return Result.Success();
	}

	public Result SetTemporaryHitPoints(int amount)
	{
		if (amount < 0)
			return HitPointsErrors.InvalidTemporaryHitPoints(amount);

		TemporaryHitPoints = Math.Max(TemporaryHitPoints, amount);
		return Result.Success();
	}

	public Result ApplyMaxHitPointReduction(int reduction)
	{
		if (reduction <= 0)
			return HitPointsErrors.InvalidMaxHitPointReduction(reduction);

		MaxHitPointReduction += reduction;
		CurrentHitPoints     =  Math.Min(CurrentHitPoints, EffectiveMaxHitPoints);
		return Result.Success();
	}

	public void RemoveMaxHitPointReduction()
	{
		MaxHitPointReduction = 0;
	}

	public Result DirectSetCurrentHitPoints(int hp)
	{
		if (hp < 0)
			return HitPointsErrors.InvalidCurrentHitPoints(hp);

		CurrentHitPoints = Math.Min(hp, EffectiveMaxHitPoints);
		return Result.Success();
	}

	public Result DirectSetTemporaryHitPoints(int hp)
	{
		if (hp < 0)
			return HitPointsErrors.InvalidTemporaryHitPoints(hp);

		TemporaryHitPoints = hp;
		return Result.Success();
	}

	public Result DirectSetBaseMaxHitPoints(int baseMaxHitPoints) => SetBaseMaxHitPoints(baseMaxHitPoints);

	public Result DirectSetMaxHitPointReduction(int reduction)
	{
		if (reduction < 0)
			return HitPointsErrors.InvalidMaxHitPointReductionValue(reduction);

		MaxHitPointReduction = reduction;
		CurrentHitPoints     = Math.Min(CurrentHitPoints, EffectiveMaxHitPoints);
		return Result.Success();
	}
}
