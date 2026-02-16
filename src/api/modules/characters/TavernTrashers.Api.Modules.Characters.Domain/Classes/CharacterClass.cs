using TavernTrashers.Api.Common.Domain.Entities;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Modules.Characters.Domain.Resources;

namespace TavernTrashers.Api.Modules.Characters.Domain.Classes;

public sealed class CharacterClass : Entity
{
	private readonly List<ResourceDefinition> _resourceDefinitions = [];
	private CharacterClass() { }

	public string Name { get; private set; } = string.Empty;
	public bool IsHomebrew { get; private set; }
	public IReadOnlyCollection<ResourceDefinition> ResourceDefinitions => _resourceDefinitions.AsReadOnly();

	public static Result<CharacterClass> Create(string name, bool isHomebrew)
	{
		if (string.IsNullOrWhiteSpace(name))
			return CharacterClassErrors.InvalidName();

		return new CharacterClass
		{
			Id         = Guid.NewGuid(),
			Name       = name.Trim(),
			IsHomebrew = isHomebrew,
		};
	}

	public Result<ResourceDefinition> AddResourceDefinition(string name)
	{
		var result = ResourceDefinition.Create(Id, name);

		if (result.IsSuccess)
			_resourceDefinitions.Add(result.Value);

		return result;
	}
}