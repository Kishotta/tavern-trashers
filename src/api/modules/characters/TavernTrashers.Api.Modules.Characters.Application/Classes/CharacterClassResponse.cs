using TavernTrashers.Api.Modules.Characters.Domain.Classes;
using TavernTrashers.Api.Modules.Characters.Domain.Resources;

namespace TavernTrashers.Api.Modules.Characters.Application.Classes;

public sealed record CharacterClassResponse(
	Guid Id,
	string Name,
	bool IsHomebrew,
	IReadOnlyCollection<ResourceDefinitionResponse> ResourceDefinitions)
{
	public static implicit operator CharacterClassResponse(CharacterClass characterClass) =>
		new(
			characterClass.Id,
			characterClass.Name,
			characterClass.IsHomebrew,
			characterClass.ResourceDefinitions
			   .Select(rd => (ResourceDefinitionResponse)rd)
			   .ToList()
			   .AsReadOnly());
}

public sealed record ResourceDefinitionResponse(
	Guid Id,
	string Name,
	IReadOnlyCollection<ResourceAllowanceResponse> Allowances)
{
	public static implicit operator ResourceDefinitionResponse(ResourceDefinition resourceDefinition) =>
		new(
			resourceDefinition.Id,
			resourceDefinition.Name,
			resourceDefinition.Allowances
			   .Select(a => (ResourceAllowanceResponse)a)
			   .OrderBy(a => a.Level)
			   .ToList()
			   .AsReadOnly());
}

public sealed record ResourceAllowanceResponse(int Level, int Amount)
{
	public static implicit operator ResourceAllowanceResponse(ResourceAllowance allowance) =>
		new(allowance.Level, allowance.Amount);
}
