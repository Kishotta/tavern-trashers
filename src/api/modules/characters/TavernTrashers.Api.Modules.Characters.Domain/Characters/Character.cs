using TavernTrashers.Api.Common.Domain.Auditing;
using TavernTrashers.Api.Common.Domain.Entities;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Modules.Characters.Domain.Classes;
using TavernTrashers.Api.Modules.Characters.Domain.ClassLevels;
using TavernTrashers.Api.Modules.Characters.Domain.Resources;

namespace TavernTrashers.Api.Modules.Characters.Domain.Characters;

[Auditable]
public sealed class Character : Entity
{
	private static readonly Dictionary<Condition, Condition[]> _implications = new()
	{
		{ Condition.Paralyzed,  [Condition.Incapacitated] },
		{ Condition.Petrified,  [Condition.Incapacitated] },
		{ Condition.Stunned,    [Condition.Incapacitated] },
		{ Condition.Unconscious, [Condition.Incapacitated, Condition.Prone] },
	};

	private readonly List<GenericResource> _genericResources = [];
	private Character() { }

	public string Name { get; private set; } = string.Empty;
	public int Level { get; private set; } = 1;
	public Guid OwnerId { get; private set; }
	public Guid CampaignId { get; private set; }
	public Condition Conditions { get; private set; } = Condition.None;
	public HitPoints HitPoints { get; private set; } = null!;
	public DeathSavingThrows DeathSavingThrows { get; private set; } = null!;
	public IReadOnlyCollection<GenericResource> GenericResources => _genericResources.AsReadOnly();

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

		return character;
	}

	public void ChangeName(string name) => Name = name.Trim();

	public void ApplyCondition(Condition condition)
	{
		Conditions |= condition;

		if (_implications.TryGetValue(condition, out var implied))
			foreach (var impliedCondition in implied)
				Conditions |= impliedCondition;
	}

	public void RemoveCondition(Condition condition)
	{
		Conditions &= ~condition;

		if (!_implications.TryGetValue(condition, out var implied))
			return;

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

	public Result UseGenericResource(Guid resourceId)
	{
		var resource = _genericResources.SingleOrDefault(r => r.Id == resourceId);

		if (resource is null)
			return GenericResourceErrors.NotFound(resourceId);

		return resource.Use();
	}

	public Result ApplyGenericResource(Guid resourceId)
	{
		var resource = _genericResources.SingleOrDefault(r => r.Id == resourceId);

		if (resource is null)
			return GenericResourceErrors.NotFound(resourceId);

		return resource.Apply();
	}

	public Result RestoreGenericResource(Guid resourceId)
	{
		var resource = _genericResources.SingleOrDefault(r => r.Id == resourceId);

		if (resource is null)
			return GenericResourceErrors.NotFound(resourceId);

		resource.Restore();
		return Result.Success();
	}

	public void BulkRestoreByTrigger(ResetTrigger trigger)
	{
		foreach (var resource in _genericResources.Where(r => r.HasResetTrigger(trigger)))
			resource.Restore();
	}

	public Result SetBaseMaxHitPoints(int baseMaxHitPoints) =>
		HitPoints.SetBaseMaxHitPoints(baseMaxHitPoints);

	public Result TakeDamage(int amount)
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

		// Knocked from positive HP to 0: reset death saving throws
		if (previousHp > 0 && HitPoints.CurrentHitPoints == 0)
			DeathSavingThrows.Reset();

		return result;
	}

	public Result Heal(int amount)
	{
		var result = HitPoints.Heal(amount);
		if (result.IsSuccess && HitPoints.CurrentHitPoints > 0)
			DeathSavingThrows.Reset();
		return result;
	}

	public Result SetTemporaryHitPoints(int amount) =>
		HitPoints.SetTemporaryHitPoints(amount);

	public Result ApplyMaxHitPointReduction(int reduction) =>
		HitPoints.ApplyMaxHitPointReduction(reduction);

	public Result RemoveMaxHitPointReduction()
	{
		HitPoints.RemoveMaxHitPointReduction();
		return Result.Success();
	}

	public Result SetHitPointFields(
		int? baseMaxHitPoints,
		int? currentHitPoints,
		int? temporaryHitPoints,
		int? maxHitPointReduction)
	{
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

		return Result.Success();
	}

	public Result RecordDeathSavingThrowSuccess() =>
		DeathSavingThrows.RecordSuccess();

	public Result RecordDeathSavingThrowFailure() =>
		DeathSavingThrows.RecordFailure();

	public void ResetDeathSavingThrows() =>
		DeathSavingThrows.Reset();
}
