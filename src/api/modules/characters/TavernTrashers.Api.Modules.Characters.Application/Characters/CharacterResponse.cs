using TavernTrashers.Api.Modules.Characters.Domain.Characters;
using TavernTrashers.Api.Modules.Characters.Domain.ClassLevels;
using TavernTrashers.Api.Modules.Characters.Domain.Resources;

namespace TavernTrashers.Api.Modules.Characters.Application.Characters;

public sealed record CharacterResponse(
	Guid Id,
	string Name,
	IReadOnlyCollection<ClassLevelResponse> ClassLevels,
	IReadOnlyCollection<CharacterResourceResponse> Resources)
{
	public static implicit operator CharacterResponse(Character character) =>
		new(
			character.Id,
			character.Name,
			character.ClassLevels.Select(cl => (ClassLevelResponse)cl).ToList().AsReadOnly(),
			character.Resources.Select(r => (CharacterResourceResponse)r).ToList().AsReadOnly());
}

public sealed record ClassLevelResponse(
	Guid Id,
	Guid CharacterClassId,
	string ClassName,
	int Level)
{
	public static implicit operator ClassLevelResponse(ClassLevel classLevel) =>
		new(
			classLevel.Id,
			classLevel.CharacterClassId,
			classLevel.CharacterClass?.Name ?? string.Empty,
			classLevel.Level);
}

public sealed record CharacterResourceResponse(
	Guid Id,
	Guid ResourceDefinitionId,
	string ResourceName,
	int CurrentAmount,
	int MaxAmount)
{
	public static implicit operator CharacterResourceResponse(CharacterResource resource) =>
		new(
			resource.Id,
			resource.ResourceDefinitionId,
			resource.ResourceDefinition?.Name ?? string.Empty,
			resource.CurrentAmount,
			resource.MaxAmount);
}
