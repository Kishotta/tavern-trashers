using TavernTrashers.Api.Common.Domain.Entities;
using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Modules.Characters.Domain.Resources;

public sealed class ResourceDefinition : Entity
{
	private readonly List<ResourceAllowance> _allowances = [];
	private ResourceDefinition() { }

	public Guid CharacterClassId { get; private set; }
	public string Name { get; private set; } = string.Empty;
	public IReadOnlyCollection<ResourceAllowance> Allowances => _allowances.AsReadOnly();

	public int GetAmountAtLevel(int level) =>
		_allowances
		   .Where(a => a.Level <= level)
		   .OrderByDescending(a => a.Level)
		   .Select(a => a.Amount)
		   .FirstOrDefault();

	public static Result<ResourceDefinition> Create(Guid characterClassId, string name)
	{
		if (string.IsNullOrWhiteSpace(name))
			return CharacterResourceErrors.InvalidName();

		return new ResourceDefinition
		{
			Id               = Guid.NewGuid(),
			CharacterClassId = characterClassId,
			Name             = name.Trim(),
		};
	}

	public void AddAllowance(int level, int amount) =>
		_allowances.Add(ResourceAllowance.Create(level, amount));
}