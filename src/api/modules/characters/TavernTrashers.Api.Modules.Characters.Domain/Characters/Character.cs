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
	private Character() { }

	public string Name { get; private set; } = string.Empty;
	public IReadOnlyCollection<ClassLevel> ClassLevels => _classLevels.AsReadOnly();
	public IReadOnlyCollection<CharacterResource> Resources => _resources.AsReadOnly();

	public static Result<Character> Create(string name)
	{
		if (string.IsNullOrWhiteSpace(name))
			return CharacterErrors.InvalidName();

		return new Character
		{
			Id   = Guid.NewGuid(),
			Name = name.Trim(),
		};
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