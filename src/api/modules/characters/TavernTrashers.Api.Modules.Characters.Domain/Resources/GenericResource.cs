using TavernTrashers.Api.Common.Domain.Entities;
using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Modules.Characters.Domain.Resources;

public sealed class GenericResource : Entity
{
	private GenericResource() { }

	public Guid CharacterId { get; private set; }
	public string Name { get; private set; } = string.Empty;
	public int CurrentUses { get; private set; }
	public int MaxUses { get; private set; }
	public ResourceDirection Direction { get; private set; }
	public SourceCategory SourceCategory { get; private set; }
	public string ResetTriggers { get; private set; } = string.Empty;

	public IReadOnlyCollection<ResetTrigger> GetResetTriggers() =>
		ResetTriggers
		   .Split(',', StringSplitOptions.RemoveEmptyEntries)
		   .Select(Enum.Parse<ResetTrigger>)
		   .ToList()
		   .AsReadOnly();

	public Result Use()
	{
		if (CurrentUses <= 0)
			return GenericResourceErrors.AlreadyEmpty(Id);

		CurrentUses--;
		return Result.Success();
	}

	public Result Apply()
	{
		if (CurrentUses >= MaxUses)
			return GenericResourceErrors.AlreadyFull(Id);

		CurrentUses++;
		return Result.Success();
	}

	public void Restore()
	{
		CurrentUses = Direction switch
		{
			ResourceDirection.Spending     => MaxUses,
			ResourceDirection.Accumulating => 0,
			_                              => CurrentUses,
		};
	}

	public bool HasResetTrigger(ResetTrigger trigger) =>
		GetResetTriggers().Contains(trigger);

	public static Result<GenericResource> Create(
		Guid characterId,
		string name,
		int maxUses,
		ResourceDirection direction,
		SourceCategory sourceCategory,
		IEnumerable<ResetTrigger> resetTriggers)
	{
		if (string.IsNullOrWhiteSpace(name))
			return GenericResourceErrors.InvalidName();

		if (maxUses < 0)
			return GenericResourceErrors.InvalidMaxUses();

		var triggers = resetTriggers.ToList();
		var resource = new GenericResource
		{
			Id             = Guid.NewGuid(),
			CharacterId    = characterId,
			Name           = name.Trim(),
			MaxUses        = maxUses,
			Direction      = direction,
			SourceCategory = sourceCategory,
			ResetTriggers  = string.Join(',', triggers.Select(t => t.ToString())),
		};

		resource.CurrentUses = direction == ResourceDirection.Spending ? maxUses : 0;

		return resource;
	}
}
