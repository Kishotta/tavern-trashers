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
	private readonly List<ClassLevel> _classLevels = [];
	private readonly List<CharacterResource> _resources = [];
	private readonly List<GenericResource> _genericResources = [];
	private Character() { }

	public string Name { get; private set; } = string.Empty;
	public int Level { get; private set; } = 1;
	public Guid OwnerId { get; private set; }
	public Guid CampaignId { get; private set; }
	public HpTracker? HpTracker { get; private set; }
	public IReadOnlyCollection<ClassLevel> ClassLevels => _classLevels.AsReadOnly();
	public IReadOnlyCollection<CharacterResource> Resources => _resources.AsReadOnly();
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

		return character;
	}

	public void ChangeName(string name) => Name = name.Trim();

	public Result SetClassLevel(CharacterClass characterClass, int level)
	{
		if (level is < 1 or > 20)
			return CharacterErrors.InvalidLevel(level);

		var existing = _classLevels.SingleOrDefault(cl => cl.CharacterClassId == characterClass.Id);

		if (existing is not null)
			existing.SetLevel(level);
		else
			_classLevels.Add(ClassLevel.Create(Id, characterClass.Id, level));

		RecalculateResources();
		return Result.Success();
	}

	public Result RemoveClassLevel(Guid characterClassId)
	{
		var existing = _classLevels.SingleOrDefault(cl => cl.CharacterClassId == characterClassId);

		if (existing is null)
			return CharacterErrors.ClassLevelNotFound(characterClassId);

		_classLevels.Remove(existing);
		RecalculateResources();
		return Result.Success();
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

	public Result InitializeHpTracker(int baseMaxHp)
	{
		var result = HpTracker.Create(Id, baseMaxHp);

		if (result.IsSuccess)
			HpTracker = result.Value;

		return result;
	}

	public Result SetBaseMaxHp(int baseMaxHp)
	{
		if (HpTracker is null)
			return InitializeHpTracker(baseMaxHp);

		return HpTracker.SetBaseMaxHp(baseMaxHp);
	}

	public Result TakeDamage(int amount)
	{
		if (HpTracker is null)
			return HpTrackerErrors.NotFound(Id);

		return HpTracker.TakeDamage(amount);
	}

	public Result Heal(int amount)
	{
		if (HpTracker is null)
			return HpTrackerErrors.NotFound(Id);

		return HpTracker.Heal(amount);
	}

	public Result SetTemporaryHp(int amount)
	{
		if (HpTracker is null)
			return HpTrackerErrors.NotFound(Id);

		return HpTracker.SetTemporaryHp(amount);
	}

	public Result ApplyMaxHpReduction(int reduction)
	{
		if (HpTracker is null)
			return HpTrackerErrors.NotFound(Id);

		return HpTracker.ApplyMaxHpReduction(reduction);
	}

	public Result RemoveMaxHpReduction()
	{
		if (HpTracker is null)
			return HpTrackerErrors.NotFound(Id);

		HpTracker.RemoveMaxHpReduction();
		return Result.Success();
	}

	public Result SetHpFields(int? baseMaxHp, int? currentHp, int? temporaryHp, int? maxHpReduction)
	{
		if (HpTracker is null)
			return HpTrackerErrors.NotFound(Id);

		if (baseMaxHp.HasValue)
		{
			var result = HpTracker.DirectSetBaseMaxHp(baseMaxHp.Value);
			if (result.IsFailure) return result;
		}

		if (maxHpReduction.HasValue)
		{
			var result = HpTracker.DirectSetMaxHpReduction(maxHpReduction.Value);
			if (result.IsFailure) return result;
		}

		if (currentHp.HasValue)
		{
			var result = HpTracker.DirectSetCurrentHp(currentHp.Value);
			if (result.IsFailure) return result;
		}

		if (temporaryHp.HasValue)
		{
			var result = HpTracker.DirectSetTemporaryHp(temporaryHp.Value);
			if (result.IsFailure) return result;
		}

		return Result.Success();
	}

	private void RecalculateResources()
	{
		var expectedResources = new Dictionary<Guid, int>();

		foreach (var classLevel in _classLevels)
		{
			if (classLevel.CharacterClass?.ResourceDefinitions is null)
				continue;

			foreach (var resourceDef in classLevel.CharacterClass.ResourceDefinitions)
			{
				var amount = resourceDef.GetAmountAtLevel(classLevel.Level);
				if (amount > 0)
					expectedResources[resourceDef.Id] = amount;
			}
		}

		// Remove resources that no longer apply
		_resources.RemoveAll(r => !expectedResources.ContainsKey(r.ResourceDefinitionId));

		// Add or update resources
		foreach (var (resourceDefId, maxAmount) in expectedResources)
		{
			var existing = _resources.SingleOrDefault(r => r.ResourceDefinitionId == resourceDefId);

			if (existing is not null)
				existing.SetMax(maxAmount);
			else
				_resources.Add(CharacterResource.Create(Id, resourceDefId, maxAmount));
		}
	}
}
