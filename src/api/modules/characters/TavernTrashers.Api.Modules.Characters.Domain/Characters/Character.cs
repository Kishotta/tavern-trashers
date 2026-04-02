using TavernTrashers.Api.Common.Domain.Auditing;
using TavernTrashers.Api.Common.Domain.Entities;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Modules.Characters.Domain.Characters.Events;
using TavernTrashers.Api.Modules.Characters.Domain.Classes;
using TavernTrashers.Api.Modules.Characters.Domain.ClassLevels;
using TavernTrashers.Api.Modules.Characters.Domain.Resources;

namespace TavernTrashers.Api.Modules.Characters.Domain.Characters;

[Auditable]
public sealed class Character : Entity
{
	private static readonly Dictionary<Conditions, Conditions[]> _implications = new()
	{
		{ Conditions.Paralyzed,  [Conditions.Incapacitated] },
		{ Conditions.Petrified,  [Conditions.Incapacitated] },
		{ Conditions.Stunned,    [Conditions.Incapacitated] },
		{ Conditions.Unconscious, [Conditions.Incapacitated, Conditions.Prone] },
	};

	private readonly List<GenericResource> _genericResources = [];
	private readonly List<SpellSlotPool> _spellSlotPools = [];
	private Character() { }

	public string Name { get; private set; } = string.Empty;
	public int Level { get; private set; } = 1;
	public Guid OwnerId { get; private set; }
	public Guid CampaignId { get; private set; }
	public Conditions Conditions { get; private set; } = Conditions.None;
	public HitPoints HitPoints { get; private set; } = null!;
	public DeathSavingThrows DeathSavingThrows { get; private set; } = null!;
	public IReadOnlyCollection<GenericResource> GenericResources => _genericResources.AsReadOnly();
	public IReadOnlyCollection<SpellSlotPool> SpellSlotPools => _spellSlotPools.AsReadOnly();

	public static Result<Character> Create(string name, int level, Guid ownerId, Guid campaignId)
	{
		if (string.IsNullOrWhiteSpace(name))
			return CharacterErrors.InvalidName();

		if (level is < 1 or > 20)
			return CharacterErrors.InvalidLevel(level);

		var character = new Character
		{
			Id         = Guid.NewGuid(),
			Name       = name.Trim(),
			Level      = level,
			OwnerId    = ownerId,
			CampaignId = campaignId,
		};

		character._genericResources.AddRange(DefaultResourceFactory.CreateDefaultResources(character.Id));
		character.HitPoints           = HitPoints.CreateDefault(character.Id);
		character.DeathSavingThrows   = DeathSavingThrows.CreateDefault(character.Id);
		character._spellSlotPools.Add(SpellSlotPool.CreateDefault(character.Id));

		return character;
	}

	public void ChangeName(string name) => Name = name.Trim();

	private void RaiseResourceChanged(string resourceName, string oldValue, string newValue, string actor) =>
		RaiseDomainEvent(new ResourceChangedDomainEvent(Id, Name, CampaignId, resourceName, oldValue, newValue, actor));

	public void ApplyCondition(Conditions condition, string actor = "")
	{
		var oldConditions = Conditions;
		Conditions |= condition;

		if (_implications.TryGetValue(condition, out var implied))
			foreach (var impliedCondition in implied)
				Conditions |= impliedCondition;

		RaiseResourceChanged("Conditions", oldConditions.ToString(), Conditions.ToString(), actor);
	}

	public void RemoveCondition(Conditions condition, string actor = "")
	{
		var oldConditions = Conditions;
		Conditions &= ~condition;

		if (_implications.TryGetValue(condition, out var implied))
		{
			foreach (var impliedCondition in implied)
			{
				var stillImplied = _implications.Any(kvp =>
					kvp.Key != condition &&
					Conditions.HasFlag(kvp.Key) &&
					kvp.Value.Contains(impliedCondition));

				if (!stillImplied)
					Conditions &= ~impliedCondition;
			}
		}

		RaiseResourceChanged("Conditions", oldConditions.ToString(), Conditions.ToString(), actor);
	}

	public Result<GenericResource> AddGenericResource(
		string name,
		int maxUses,
		ResourceDirection direction,
		SourceCategory sourceCategory,
		IEnumerable<ResetTrigger> resetTriggers)
	{
		var result = GenericResource.Create(Id, name, maxUses, direction, sourceCategory, resetTriggers);

		if (result.IsSuccess)
			_genericResources.Add(result.Value);

		return result;
	}

	public Result RemoveGenericResource(Guid resourceId)
	{
		var resource = _genericResources.SingleOrDefault(r => r.Id == resourceId);

		if (resource is null)
			return GenericResourceErrors.NotFound(resourceId);

		_genericResources.Remove(resource);
		return Result.Success();
	}

	public Result UseGenericResource(Guid resourceId, string actor = "")
	{
		var resource = _genericResources.SingleOrDefault(r => r.Id == resourceId);

		if (resource is null)
			return GenericResourceErrors.NotFound(resourceId);

		var oldUses = resource.CurrentUses;
		var result = resource.Use();

		if (result.IsSuccess)
			RaiseResourceChanged(resource.Name, $"{oldUses}/{resource.MaxUses}", $"{resource.CurrentUses}/{resource.MaxUses}", actor);

		return result;
	}

	public Result ApplyGenericResource(Guid resourceId, string actor = "")
	{
		var resource = _genericResources.SingleOrDefault(r => r.Id == resourceId);

		if (resource is null)
			return GenericResourceErrors.NotFound(resourceId);

		var oldUses = resource.CurrentUses;
		var result = resource.Apply();

		if (result.IsSuccess)
			RaiseResourceChanged(resource.Name, $"{oldUses}/{resource.MaxUses}", $"{resource.CurrentUses}/{resource.MaxUses}", actor);

		return result;
	}

	public Result RestoreGenericResource(Guid resourceId, string actor = "")
	{
		var resource = _genericResources.SingleOrDefault(r => r.Id == resourceId);

		if (resource is null)
			return GenericResourceErrors.NotFound(resourceId);

		var oldUses = resource.CurrentUses;
		resource.Restore();
		RaiseResourceChanged(resource.Name, $"{oldUses}/{resource.MaxUses}", $"{resource.CurrentUses}/{resource.MaxUses}", actor);
		return Result.Success();
	}

	public void BulkRestoreByTrigger(ResetTrigger trigger, string actor = "")
	{
		foreach (var resource in _genericResources.Where(r => r.HasResetTrigger(trigger)))
			resource.Restore();

		foreach (var pool in _spellSlotPools.Where(p => p.GetResetTrigger() == trigger))
			pool.Restore();

		RaiseResourceChanged($"{trigger} Restore", "used", "restored", actor);
	}

	public Result AddPactMagicSlotPool()
	{
		if (_spellSlotPools.Any(p => p.Kind == SpellSlotPoolKind.PactMagic))
			return SpellSlotPoolErrors.PactMagicAlreadyExists();

		_spellSlotPools.Add(SpellSlotPool.CreatePactMagic(Id));
		return Result.Success();
	}

	public Result UseSpellSlot(Guid poolId, int level, string actor = "")
	{
		var pool = _spellSlotPools.SingleOrDefault(p => p.Id == poolId);
		if (pool is null)
			return SpellSlotPoolErrors.NotFound(poolId);

		var slot = pool.Levels.SingleOrDefault(l => l.Level == level);
		var oldUses = slot?.CurrentUses ?? 0;
		var maxUses = slot?.MaxUses ?? 0;

		var result = pool.UseSlot(level);

		if (result.IsSuccess && slot is not null)
			RaiseResourceChanged($"Level {level} Spell Slots", $"{oldUses}/{maxUses}", $"{slot.CurrentUses}/{maxUses}", actor);

		return result;
	}

	public Result RestoreSlotPool(Guid poolId, string actor = "")
	{
		var pool = _spellSlotPools.SingleOrDefault(p => p.Id == poolId);
		if (pool is null)
			return SpellSlotPoolErrors.NotFound(poolId);

		pool.Restore();
		RaiseResourceChanged($"{pool.Kind} Spell Slots", "used", "restored", actor);
		return Result.Success();
	}

	public Result SetMaxSpellSlots(Guid poolId, int level, int max)
	{
		var pool = _spellSlotPools.SingleOrDefault(p => p.Id == poolId);
		if (pool is null)
			return SpellSlotPoolErrors.NotFound(poolId);

		return pool.SetMaxSlots(level, max);
	}

	public Result SetBaseMaxHitPoints(int baseMaxHitPoints, string actor = "")
	{
		var oldMax = HitPoints.BaseMaxHitPoints;
		var result = HitPoints.SetBaseMaxHitPoints(baseMaxHitPoints);

		if (result.IsSuccess)
			RaiseResourceChanged("Max Hit Points", oldMax.ToString(), HitPoints.BaseMaxHitPoints.ToString(), actor);

		return result;
	}

	public Result TakeDamage(int amount, string actor = "")
	{
		// At 0 HP: nonzero damage auto-records 1 death saving throw failure (unless already at max)
		if (HitPoints.CurrentHitPoints == 0 && amount > 0)
		{
			if (DeathSavingThrows.Failures < 3)
				DeathSavingThrows.RecordFailure();
			return Result.Success();
		}

		var previousHp = HitPoints.CurrentHitPoints;
		var result     = HitPoints.TakeDamage(amount);
		if (result.IsFailure) return result;

		RaiseResourceChanged("Hit Points", $"{previousHp}/{HitPoints.EffectiveMaxHitPoints}", $"{HitPoints.CurrentHitPoints}/{HitPoints.EffectiveMaxHitPoints}", actor);

		// Knocked from positive HP to 0: reset death saving throws
		if (previousHp > 0 && HitPoints.CurrentHitPoints == 0)
			DeathSavingThrows.Reset();

		return result;
	}

	public Result Heal(int amount, string actor = "")
	{
		var previousHp = HitPoints.CurrentHitPoints;
		var result = HitPoints.Heal(amount);

		if (result.IsSuccess)
		{
			RaiseResourceChanged("Hit Points", $"{previousHp}/{HitPoints.EffectiveMaxHitPoints}", $"{HitPoints.CurrentHitPoints}/{HitPoints.EffectiveMaxHitPoints}", actor);
			if (HitPoints.CurrentHitPoints > 0)
				DeathSavingThrows.Reset();
		}

		return result;
	}

	public Result SetTemporaryHitPoints(int amount, string actor = "")
	{
		var oldTemp = HitPoints.TemporaryHitPoints;
		var result = HitPoints.SetTemporaryHitPoints(amount);

		if (result.IsSuccess)
			RaiseResourceChanged("Temporary Hit Points", oldTemp.ToString(), HitPoints.TemporaryHitPoints.ToString(), actor);

		return result;
	}

	public Result ApplyMaxHitPointReduction(int reduction, string actor = "")
	{
		var oldReduction = HitPoints.MaxHitPointReduction;
		var result = HitPoints.ApplyMaxHitPointReduction(reduction);

		if (result.IsSuccess)
			RaiseResourceChanged("Max HP Reduction", oldReduction.ToString(), HitPoints.MaxHitPointReduction.ToString(), actor);

		return result;
	}

	public Result RemoveMaxHitPointReduction(string actor = "")
	{
		var oldReduction = HitPoints.MaxHitPointReduction;
		HitPoints.RemoveMaxHitPointReduction();
		RaiseResourceChanged("Max HP Reduction", oldReduction.ToString(), HitPoints.MaxHitPointReduction.ToString(), actor);
		return Result.Success();
	}

	public Result SetHitPointFields(
		int? baseMaxHitPoints,
		int? currentHitPoints,
		int? temporaryHitPoints,
		int? maxHitPointReduction,
		string actor = "")
	{
		var oldHp  = HitPoints.CurrentHitPoints;
		var oldMax = HitPoints.EffectiveMaxHitPoints;

		if (baseMaxHitPoints.HasValue)
		{
			var result = HitPoints.DirectSetBaseMaxHitPoints(baseMaxHitPoints.Value);
			if (result.IsFailure) return result;
		}

		if (maxHitPointReduction.HasValue)
		{
			var result = HitPoints.DirectSetMaxHitPointReduction(maxHitPointReduction.Value);
			if (result.IsFailure) return result;
		}

		if (currentHitPoints.HasValue)
		{
			var result = HitPoints.DirectSetCurrentHitPoints(currentHitPoints.Value);
			if (result.IsFailure) return result;
		}

		if (temporaryHitPoints.HasValue)
		{
			var result = HitPoints.DirectSetTemporaryHitPoints(temporaryHitPoints.Value);
			if (result.IsFailure) return result;
		}

		if (HitPoints.CurrentHitPoints > 0)
			DeathSavingThrows.Reset();

		RaiseResourceChanged("Hit Points", $"{oldHp}/{oldMax}", $"{HitPoints.CurrentHitPoints}/{HitPoints.EffectiveMaxHitPoints}", actor);

		return Result.Success();
	}

	public Result RecordDeathSavingThrowSuccess(string actor = "")
	{
		var oldSuccesses = DeathSavingThrows.Successes;
		var result = DeathSavingThrows.RecordSuccess();

		if (result.IsSuccess)
			RaiseResourceChanged("Death Saving Throws", $"S:{oldSuccesses}/F:{DeathSavingThrows.Failures}", $"S:{DeathSavingThrows.Successes}/F:{DeathSavingThrows.Failures}", actor);

		return result;
	}

	public Result RecordDeathSavingThrowFailure(string actor = "")
	{
		var oldFailures = DeathSavingThrows.Failures;
		var result = DeathSavingThrows.RecordFailure();

		if (result.IsSuccess)
			RaiseResourceChanged("Death Saving Throws", $"S:{DeathSavingThrows.Successes}/F:{oldFailures}", $"S:{DeathSavingThrows.Successes}/F:{DeathSavingThrows.Failures}", actor);

		return result;
	}

	public void ResetDeathSavingThrows(string actor = "")
	{
		var oldSuccesses = DeathSavingThrows.Successes;
		var oldFailures  = DeathSavingThrows.Failures;
		DeathSavingThrows.Reset();
		RaiseResourceChanged("Death Saving Throws", $"S:{oldSuccesses}/F:{oldFailures}", $"S:{DeathSavingThrows.Successes}/F:{DeathSavingThrows.Failures}", actor);
	}
}
